Imports System
Imports Microsoft.SPOT.Hardware
Imports Microsoft.SPOT

Public Class nRF24L01P
    Implements IDisposable

    ''' <summary>
    ''' Occurs when data is received.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event OnDataReceived(sender As Object, e As DataReceivedEventArgs)

    ''' <summary>
    ''' Occurs when data is received with bad payload.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event OnDataReceiveFailed(sender As Object, e As DataReceiveFailedEventArgs)


    ''' <summary>
    ''' Occurs when data is received.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event OnTransmitFailed(sender As Object, e As DataTransmitFailedEventArgs)

    ''' <summary>
    ''' Occurs when data is successfully transmitted.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Event OnTransmitSuccess(sender As Object, e As EventArgs)

    Private NordicChipEnablePin As OutputPort
    Private WithEvents NordicInterruptPin As InterruptPort
    Private NordicSPI As SPI

    Public Sub New(SPI As SPI.SPI_module, Speed As UInteger, ChipSelectPin As Cpu.Pin, ChipEnablePin As Cpu.Pin, InterruptPin As Cpu.Pin)

        NordicSPI = New SPI(New SPI.Configuration(ChipSelectPin, False, 0, 0, False, True, Speed, SPI))
        NordicInterruptPin = New InterruptPort(InterruptPin, False, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow)
        NordicChipEnablePin = New OutputPort(ChipEnablePin, False)

        '#######################################################################################
        'Vider le TX FIFO
        Dim StatusBuffer() As Byte = Nothing
        StatusBuffer = FlushTXFIFO()
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H10)})

        'Vider le RX FIFO
        FlushRXFIFO()
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H40)})

    End Sub

    Public Function ReadFIFO(width As Byte) As Byte()

        Dim writeBuffer(width) As Byte
        Dim readBuffer(width) As Byte

        writeBuffer(0) = CByte(Common.Commands.R_RX_PAYLOAD)

        NordicSPI.WriteRead(writeBuffer, readBuffer)

        Return readBuffer

    End Function

    Public Function ReadRegister(register As Common.Registers, Optional size As Byte = 1) As Byte()

        Dim writeBuffer = New Byte(size) {}
        Dim readBuffer = New Byte(size) {}

        writeBuffer(0) = CByte(Common.Commands.R_REGISTER) Or CByte(register)

        NordicSPI.WriteRead(writeBuffer, readBuffer)

        Return readBuffer

    End Function

    Public Sub WriteRegister(register As Common.Registers, data() As Byte)

        Dim writeBuffer = New Byte(data.Length) {}

        writeBuffer(0) = CByte(Common.Commands.W_REGISTER) Or CByte(register)
        Array.Copy(data, 0, writeBuffer, 1, data.Length)

        NordicSPI.Write(writeBuffer)

    End Sub

    Public Sub WritePayload(data() As Byte)

        Dim writeBuffer = New Byte(data.Length) {}

        Try
            writeBuffer(0) = Common.Commands.W_TX_PAYLOAD
            Array.Copy(data, 0, writeBuffer, 1, data.Length)

            NordicSPI.Write(writeBuffer)

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

    End Sub

    Public Function FlushTXFIFO() As Byte()

        Dim writeBuffer(0) As Byte
        Dim readBuffer(0) As Byte

        Try
            writeBuffer(0) = Common.Commands.FLUSH_TX

            NordicSPI.WriteRead(writeBuffer, readBuffer)
        Catch ex As Exception
            Debug.Print("FlushTXFIFO : " + ex.Message)
        End Try


        Return readBuffer

    End Function

    Public Sub FlushRXFIFO()

        Dim writeBuffer(0) As Byte

        Try
            writeBuffer(0) = Common.Commands.FLUSH_RX

            NordicSPI.Write(writeBuffer)

        Catch ex As Exception
            Debug.Print("FlushRXFIFO : " + ex.Message)
        End Try

    End Sub

    Public Function ReadRXFIFOWidth() As Byte

        Dim writeBuffer(1) As Byte
        Dim readBuffer(1) As Byte

        Try
            writeBuffer(0) = Common.Commands.R_RX_PL_WID

            NordicSPI.WriteRead(writeBuffer, readBuffer)
        Catch ex As Exception
            Debug.Print("ReadRXFIFOWidth : " + ex.Message)
        End Try

        Return readBuffer(1)

    End Function

    Public Sub SetRegisterBit(ByRef registerByte As Byte, value As Boolean, index As Integer)

        Try
            If value Then
                registerByte = registerByte Or CByte(2 ^ index)
            Else
                registerByte = registerByte And CByte(255 - (2 ^ index))
            End If
        Catch ex As Exception
            Debug.Print("SetRegisterBit : " + ex.Message)
        End Try

    End Sub

    Public Sub SetChipEnable(value As Boolean)

        Try
            If value Then
                NordicChipEnablePin.Write(True)
            Else
                NordicChipEnablePin.Write(False)
            End If
        Catch ex As Exception
            Debug.Print("SetChipEnable : " + ex.Message)
        End Try

    End Sub

    Private Sub NordicInterruptPin_OnInterrupt(data1 As UInteger, data2 As UInteger, time As Date) Handles NordicInterruptPin.OnInterrupt
        'Get the STATUS Register
        Dim writeBuffer(1) As Byte
        Dim StatusBuffer(1) As Byte

        writeBuffer(0) = Common.Commands.NOP
        NordicSPI.WriteRead(writeBuffer, StatusBuffer)

        '########################################################
        'Vérifier les bits d'état du status

        If (StatusBuffer(0) And &H10) = &H10 Then    'MAX_RT : Maximum number of retransmit reached
            FlushTXFIFO()
            RaiseEvent OnTransmitFailed(Me, New DataTransmitFailedEventArgs(StatusBuffer(0)))

        End If

        If (StatusBuffer(0) And &H20) = &H20 Then 'TX_DS : Données envoyées
            RaiseEvent OnTransmitSuccess(Me, EventArgs.Empty)

        End If

        If (StatusBuffer(0) And &H40) = &H40 Then 'RX_DR : Données disponible dans le FIFO

            Dim pipeID As Byte
            Dim PayloadLen As Byte

            While True
                'Obtenir le PIPE_ID
                pipeID = CByte((StatusBuffer(0) And &HE) / 2)
                PayloadLen = ReadRXFIFOWidth()
                Dim Payload(PayloadLen - 1) As Byte

                If PayloadLen > 32 Then
                    FlushRXFIFO()
                    RaiseEvent OnDataReceiveFailed(Me, New DataReceiveFailedEventArgs(pipeID, StatusBuffer(0)))

                Else
                    Array.Copy(ReadFIFO(PayloadLen), 1, Payload, 0, PayloadLen)
                    RaiseEvent OnDataReceived(Me, New DataReceivedEventArgs(pipeID, Payload))

                End If

                Dim FIFOStatusBuf(1) As Byte

                FIFOStatusBuf = ReadRegister(Common.Registers.FIFO_STATUS)

                If (FIFOStatusBuf(1) And &H1) = &H1 Then
                    Exit While
                End If

            End While

        End If

        SetRegisterBit(StatusBuffer(0), True, Common.BitFlags.MAX_RT)
        SetRegisterBit(StatusBuffer(0), True, Common.BitFlags.TX_DS)
        SetRegisterBit(StatusBuffer(0), True, Common.BitFlags.RX_DR)
        WriteRegister(Common.Registers.STATUS, New Byte() {StatusBuffer(0)})

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Pour détecter les appels redondants

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: supprimez l'état managé (objets managés).
            End If

            ' TODO: libérez les ressources non managées (objets non managés) et substituez la méthode Finalize() ci-dessous.
            ' TODO: définissez les champs volumineux à null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: substituez Finalize() uniquement si Dispose(ByVal disposing As Boolean) ci-dessus comporte du code permettant de libérer des ressources non managées.
    'Protected Overrides Sub Finalize()
    '    ' Ne modifiez pas ce code. Ajoutez du code de nettoyage dans Dispose(ByVal disposing As Boolean) ci-dessus.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Ce code a été ajouté par Visual Basic pour permettre l'implémentation correcte du modèle pouvant être supprimé.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ne modifiez pas ce code. Ajoutez du code de nettoyage dans Dispose(disposing As Boolean) ci-dessus.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "EventArgs"


    Public Class DataReceivedEventArgs
        Inherits EventArgs

        Public DataPipe As Byte
        Public Payload As Byte() = Nothing

        Public Sub New(DataPipe As Byte, Payload As Byte())
            Me.DataPipe = DataPipe
            Me.Payload = Payload
        End Sub

    End Class

    Public Class DataReceiveFailedEventArgs
        Inherits EventArgs

        Public DataPipe As Byte
        Public Status As Byte

        Public Sub New(DataPipe As Byte, Status As Byte)
            Me.DataPipe = DataPipe
            Me.Status = Status
        End Sub

    End Class

    Public Class DataTransmitFailedEventArgs
        Inherits EventArgs

        Public Status As Byte

        Public Sub New(Status As Byte)
            Me.Status = Status
        End Sub

    End Class

#End Region


End Class
