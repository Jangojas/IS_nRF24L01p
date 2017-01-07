Imports Windows.Devices.Gpio
Imports Windows.Devices.Spi

Public Class nRF24L01P
    Implements IDisposable

#Region "Events"

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

#End Region

#Region "Properties"

    ''' <summary>
    ''' RF channel frequency. It determines the center of the channel used by the nRF24L01p. 
    ''' Number from 0 to 125 are allowed.
    ''' </summary>
    ''' <returns></returns>
    Public Property ChannelFreq As Byte
        Get
            Return m_ChannelFreq
        End Get
        Set(value As Byte)
            If value > 125 Then
                m_ChannelFreq = 125
            ElseIf value < 0 Then
                m_ChannelFreq = 0
            Else : m_ChannelFreq = value
            End If

            WriteRegister(Common.Registers.RF_CH, New Byte() {m_ChannelFreq})

        End Set
    End Property
    Private m_ChannelFreq As Byte

    ''' <summary>
    ''' RF address width.
    ''' </summary>
    ''' <returns></returns>
    Public Property AddressWidth As Common.AddressWidth
        Get
            Return m_AddressWidth
        End Get
        Set(value As Common.AddressWidth)
            If value > 5 Then
                m_AddressWidth = Common.AddressWidth.ADR_5
            ElseIf value < 3 Then
                m_AddressWidth = Common.AddressWidth.ADR_3
            Else : m_AddressWidth = value
            End If

            Dim configByte As Byte
            configByte = 0

            Select Case m_AddressWidth
                Case Common.AddressWidth.ADR_3
                    SetRegisterBit(configByte, False, Common.BitFlags.AW_LOW)
                    SetRegisterBit(configByte, True, Common.BitFlags.AW_HIGH)
                Case Common.AddressWidth.ADR_4
                    SetRegisterBit(configByte, True, Common.BitFlags.AW_LOW)
                    SetRegisterBit(configByte, False, Common.BitFlags.AW_HIGH)
                Case Common.AddressWidth.ADR_5
                    SetRegisterBit(configByte, True, Common.BitFlags.AW_LOW)
                    SetRegisterBit(configByte, True, Common.BitFlags.AW_HIGH)
            End Select

            WriteRegister(Common.Registers.SETUP_AW, New Byte() {configByte})

        End Set
    End Property
    Private m_AddressWidth As Common.AddressWidth

    ''' <summary>
    ''' RF db Power
    ''' </summary>
    ''' <returns></returns>
    Public Property dbPower As Common.dbPower
        Get
            Return m_dbPower
        End Get
        Set(value As Common.dbPower)

            If value > Common.dbPower.PWR_0db Then
                m_dbPower = Common.dbPower.PWR_0db
            ElseIf value < Common.dbPower.PWR_minus_18db Then
                m_dbPower = Common.dbPower.PWR_minus_18db
            Else : m_dbPower = value
            End If

            Dim configByte As Byte()
            configByte = ReadRegister(Common.Registers.RF_SETUP)

            Select Case m_dbPower
                Case Common.dbPower.PWR_minus_18db  '-18dBm
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_PWR_LOW)
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_PWR_HIGH)
                Case Common.dbPower.PWR_minus_12db   '-12dBm
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_PWR_LOW)
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_PWR_HIGH)
                Case Common.dbPower.PWR_minus_6db   '-6dBm
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_PWR_LOW)
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_PWR_HIGH)
                Case Common.dbPower.PWR_0db   '0dBm
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_PWR_LOW)
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_PWR_HIGH)
            End Select

            WriteRegister(Common.Registers.RF_SETUP, New Byte() {configByte(0)})

        End Set
    End Property
    Private m_dbPower As Common.dbPower

    ''' <summary>
    ''' RF Speed
    ''' </summary>
    ''' <returns></returns>
    Public Property Speed As Common.RFSpeed
        Get
            Return m_Speed
        End Get
        Set(value As Common.RFSpeed)

            If value > Common.RFSpeed.HIGH_2000 Then
                m_Speed = Common.RFSpeed.HIGH_2000
            ElseIf value < Common.RFSpeed.LOW_250 Then
                m_Speed = Common.RFSpeed.LOW_250
            Else : m_Speed = value
            End If

            Dim configByte As Byte()
            configByte = ReadRegister(Common.Registers.RF_SETUP)

            Select Case m_Speed
                Case Common.RFSpeed.LOW_250  '250Kbps
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_DR_LOW)
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_DR_HIGH)

                Case Common.RFSpeed.MID_1000   '1Mbps
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_DR_LOW)
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_DR_HIGH)

                Case Common.RFSpeed.HIGH_2000   '2Mbps
                    SetRegisterBit(configByte(0), False, Common.BitFlags.RF_DR_LOW)
                    SetRegisterBit(configByte(0), True, Common.BitFlags.RF_DR_HIGH)
            End Select

            WriteRegister(Common.Registers.RF_SETUP, New Byte() {configByte(0)})

        End Set
    End Property
    Private m_Speed As Common.RFSpeed

    ''' <summary>
    ''' Pipe 0 address. 5 bytes maximum.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_0_Address As Byte()
        Get
            Return m_Pipe_0_Address
        End Get
        Set(value As Byte())

            If value.Length > 5 Then
                'Copy only the first 5 bytes
                Dim buffer(4) As Byte
                Array.Copy(value, buffer, 5)
                m_Pipe_0_Address = buffer
            Else
                m_Pipe_0_Address = value
            End If

        End Set
    End Property
    Private m_Pipe_0_Address As Byte()

    ''' <summary>
    ''' Pipe 1 address. 5 bytes maximum.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_1_Address As Byte()
        Get
            Return m_Pipe_1_Address
        End Get
        Set(value As Byte())

            If value.Length > 5 Then
                'Copy only the first 5 bytes
                Dim buffer(4) As Byte
                Array.Copy(value, buffer, 5)
                m_Pipe_1_Address = buffer
            Else
                m_Pipe_1_Address = value
            End If

        End Set
    End Property
    Private m_Pipe_1_Address As Byte()

    ''' <summary>
    ''' Pipe 2 address. Takes only the LSB. Data pipes 1-5 share the most significant address bytes.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_2_Address As Byte()
        Get
            Return m_Pipe_2_Address
        End Get
        Set(value As Byte())

            'Data pipes 1-5 share the four most significant address bytes.
            Dim buffer(Pipe_1_Address.Length - 1) As Byte
            Array.Copy(Pipe_1_Address, buffer, buffer.Length)
            m_Pipe_2_Address = buffer
            'Change the LSB
            m_Pipe_2_Address(0) = value(0)

        End Set
    End Property
    Private m_Pipe_2_Address As Byte()

    ''' <summary>
    ''' Pipe 3 address. Takes only the LSB. Data pipes 1-5 share the most significant address bytes.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_3_Address As Byte()
        Get
            Return m_Pipe_3_Address
        End Get
        Set(value As Byte())

            'Data pipes 1-5 share the four most significant address bytes.
            Dim buffer(Pipe_1_Address.Length - 1) As Byte
            Array.Copy(Pipe_1_Address, buffer, buffer.Length)
            m_Pipe_3_Address = buffer
            'Change the LSB
            m_Pipe_3_Address(0) = value(0)

        End Set
    End Property
    Private m_Pipe_3_Address As Byte()

    ''' <summary>
    ''' Pipe 4 address. Takes only the LSB. Data pipes 1-5 share the most significant address bytes.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_4_Address As Byte()
        Get
            Return m_Pipe_4_Address
        End Get
        Set(value As Byte())

            'Data pipes 1-5 share the four most significant address bytes.
            Dim buffer(Pipe_1_Address.Length - 1) As Byte
            Array.Copy(Pipe_1_Address, buffer, buffer.Length)
            m_Pipe_4_Address = buffer
            'Change the LSB
            m_Pipe_4_Address(0) = value(0)

        End Set
    End Property
    Private m_Pipe_4_Address As Byte()

    ''' <summary>
    ''' Pipe 5 address. Takes only the LSB. Data pipes 1-5 share the most significant address bytes.
    ''' </summary>
    ''' <returns></returns>
    Public Property Pipe_5_Address As Byte()
        Get
            Return m_Pipe_5_Address
        End Get
        Set(value As Byte())

            'Data pipes 1-5 share the four most significant address bytes.
            Dim buffer(Pipe_1_Address.Length - 1) As Byte
            Array.Copy(Pipe_1_Address, buffer, buffer.Length)
            m_Pipe_5_Address = buffer
            'Change the LSB
            m_Pipe_5_Address(0) = value(0)

        End Set
    End Property
    Private m_Pipe_5_Address As Byte()

#End Region

    Public NordicChipEnablePin As GpioPin
    Public WithEvents NordicInterruptPin As GpioPin
    Public NordicSPI As SpiDevice

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SPI"></param>
    ''' <param name="Speed"></param>
    ''' <param name="ChipEnablePin"></param>
    ''' <param name="InterruptPin"></param>
    Public Sub New(SPI As SpiConnectionSettings, Speed As Integer, ChipEnablePin As Integer, InterruptPin As Integer)

        InitNordicSPI(SPI, Speed, ChipEnablePin, InterruptPin)

    End Sub

    Private Async Sub InitNordicSPI(SPI As SpiConnectionSettings, Speed As Integer, ChipEnablePin As Integer, InterruptPin As Integer)

        Dim myGpioController As GpioController = Await GpioController.GetDefaultAsync()
        If myGpioController Is Nothing Then
            Throw New Exception("GPIO does not exist on the current system.")
        End If

        NordicChipEnablePin = myGpioController.OpenPin(ChipEnablePin)
        NordicChipEnablePin.SetDriveMode(GpioPinDriveMode.Output)
        NordicInterruptPin = myGpioController.OpenPin(InterruptPin)
        NordicInterruptPin.SetDriveMode(GpioPinDriveMode.Input)

        SPI.ClockFrequency = Speed
        SPI.Mode = SpiMode.Mode0
        Dim mySPIController As SpiController = Await SpiController.GetDefaultAsync()
        NordicSPI = mySPIController.GetDevice(SPI)

        '#######################################################################################
        'Empty TX FIFO
        Dim StatusBuffer() As Byte = Nothing
        StatusBuffer = FlushTXFIFO()
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H10)})

        'Empty RX FIFO
        FlushRXFIFO()
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H40)})

        Pipe_0_Address = New Byte() {&H0, &H0, &H0}
        Pipe_1_Address = New Byte() {&H0, &H0, &H0}
        Pipe_2_Address = New Byte() {&H0, &H0, &H0}
        Pipe_3_Address = New Byte() {&H0, &H0, &H0}
        Pipe_4_Address = New Byte() {&H0, &H0, &H0}
        Pipe_5_Address = New Byte() {&H0, &H0, &H0}

    End Sub

    Public Function ReadFIFO(width As Byte) As Byte()

        Dim writeBuffer(width) As Byte
        Dim readBuffer(width) As Byte

        writeBuffer(0) = Common.Commands.R_RX_PAYLOAD

        NordicSPI.TransferFullDuplex(writeBuffer, readBuffer)

        Return readBuffer

    End Function

    Public Function ReadRegister(register As Common.Registers, Optional size As Byte = 1) As Byte()

        Dim writeBuffer = New Byte(size) {}
        Dim readBuffer = New Byte(size) {}

        writeBuffer(0) = Common.Commands.R_REGISTER Or register

        NordicSPI.TransferFullDuplex(writeBuffer, readBuffer)

        Return readBuffer

    End Function

    Public Sub WriteRegister(register As Common.Registers, data() As Byte)

        Dim writeBuffer = New Byte(data.Length) {}

        writeBuffer(0) = Common.Commands.W_REGISTER Or register
        Array.Copy(data, 0, writeBuffer, 1, data.Length)

        NordicSPI.Write(writeBuffer)

    End Sub

    Public Sub WritePayload(data() As Byte)

        Dim writeBuffer = New Byte(data.Length) {}

        writeBuffer(0) = Common.Commands.W_TX_PAYLOAD
        Array.Copy(data, 0, writeBuffer, 1, data.Length)

        NordicSPI.Write(writeBuffer)

    End Sub

    Public Function FlushTXFIFO() As Byte()

        Dim writeBuffer(0) As Byte
        Dim readBuffer(0) As Byte

        writeBuffer(0) = Common.Commands.FLUSH_TX

        NordicSPI.TransferFullDuplex(writeBuffer, readBuffer)

        Return readBuffer

    End Function

    Public Sub FlushRXFIFO()

        Dim writeBuffer(0) As Byte

        writeBuffer(0) = Common.Commands.FLUSH_RX

        NordicSPI.Write(writeBuffer)

    End Sub

    Public Function ReadRXFIFOWidth() As Byte

        Dim writeBuffer(1) As Byte
        Dim readBuffer(1) As Byte

        writeBuffer(0) = Common.Commands.R_RX_PL_WID

        NordicSPI.TransferFullDuplex(writeBuffer, readBuffer)

        Return readBuffer(1)

    End Function

    Public Sub SetRegisterBit(ByRef registerByte As Byte, value As Boolean, index As Integer)

        If value Then
            registerByte = registerByte Or CByte(2 ^ index)
        Else
            registerByte = registerByte And CByte(255 - (2 ^ index))
        End If

    End Sub

    Public Sub SetChipEnable(value As Boolean)

        If value Then
            NordicChipEnablePin.Write(GpioPinValue.High)
        Else
            NordicChipEnablePin.Write(GpioPinValue.Low)
        End If

    End Sub

    ''' <summary>
    ''' Set the nRF24L01p as a Transmitter
    ''' </summary>
    ''' <param name="TX_ADDR"></param>
    ''' <remarks></remarks>
    Public Sub SetAsPTX(ByRef TX_ADDR As Byte())

        Dim configByte As Byte
        '#######################################################################################
        'In PTX mode with Enhanced Shockburst and Auto Ack, data pipe 0 address must be equal to TX_ADDR.
        WriteRegister(Common.Registers.TX_ADDR, TX_ADDR)
        WriteRegister(Common.Registers.RX_ADDR_P0, TX_ADDR)

        '#######################################################################################
        'PRX (Primary Receiver)
        configByte = 0
        SetRegisterBit(configByte, False, Common.BitFlags.PRIM_RX)   'Define as transmitter
        SetRegisterBit(configByte, True, Common.BitFlags.PWR_UP)     'Initialise module
        SetRegisterBit(configByte, True, Common.BitFlags.CRCO)       'Two bytes CRC
        SetRegisterBit(configByte, True, Common.BitFlags.EN_CRC)     'Enable CRC

        WriteRegister(Common.Registers.CONFIG, New Byte() {configByte})

        '#######################################################################################
        'STATUS byte to default state
        WriteRegister(Common.Registers.STATUS, New Byte() {&HE})

        'Empty TX FIFO
        Dim StatusBuffer() As Byte = Nothing
        FlushTXFIFO()
        StatusBuffer = ReadRegister(Common.Registers.STATUS)
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H10)})

    End Sub

    ''' <summary>
    ''' Set the nRF24L01p as a Receiver
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetAsPRX()

        Dim configByte As Byte
        '#######################################################################################
        'Data Pipes address to use
        WriteRegister(Common.Registers.RX_ADDR_P0, Pipe_0_Address)
        WriteRegister(Common.Registers.RX_ADDR_P1, Pipe_1_Address)
        WriteRegister(Common.Registers.RX_ADDR_P2, Pipe_2_Address)
        WriteRegister(Common.Registers.RX_ADDR_P3, Pipe_3_Address)
        WriteRegister(Common.Registers.RX_ADDR_P4, Pipe_4_Address)
        WriteRegister(Common.Registers.RX_ADDR_P5, Pipe_5_Address)

        '#######################################################################################
        'PTX (Primary Receiver)
        configByte = 0
        SetRegisterBit(configByte, True, Common.BitFlags.PRIM_RX)    'Define as receiver
        SetRegisterBit(configByte, True, Common.BitFlags.PWR_UP)     'Initialise module
        SetRegisterBit(configByte, True, Common.BitFlags.CRCO)       'Two bytes CRC
        SetRegisterBit(configByte, True, Common.BitFlags.EN_CRC)     'Enable CRC

        WriteRegister(Common.Registers.CONFIG, New Byte() {configByte})

        '#######################################################################################
        'STATUS byte to default state
        WriteRegister(Common.Registers.STATUS, New Byte() {&HE})

        'Empty RX FIFO
        Dim StatusBuffer() As Byte = Nothing
        FlushRXFIFO()
        StatusBuffer = ReadRegister(Common.Registers.STATUS)
        WriteRegister(Common.Registers.STATUS, New Byte() {CByte(StatusBuffer(0) Or &H40)})

    End Sub

    ''' <summary>
    ''' Used to send data to another nRF24L01p enabled module
    ''' </summary>
    ''' <param name="Message">Bytes array containing the message to send</param>
    ''' <param name="TX_ADDR">Bytes array containing the address of destination device</param>
    ''' <remarks></remarks>
    Public Sub SendPayload(Message() As Byte, ByRef TX_ADDR() As Byte)

        Dim localBuffer As Byte() = Nothing
        localBuffer = ReadRegister(Common.Registers.FIFO_STATUS) 'Check transmit buffer state
        '#######################################################################################
        'Ajouter des données dans le TX FIFO
        If (localBuffer(0) And &H1) = &H1 Then 'TX_FULL : FIFO full, then empty it
            FlushTXFIFO()
        End If

        'nRF24L01p cannot send message > 32 bytes.
        For i = 1 To CInt(System.Math.Ceiling(Message.Length / 32))
            Dim buflen As Integer
            If i * 32 < Message.Length Then
                buflen = 31
            Else
                buflen = Message.Length - ((i - 1) * 32) - 1
            End If
            Dim buffer(buflen) As Byte
            Array.Copy(Message, ((i - 1) * 32), buffer, 0, buflen + 1)
            SetChipEnable(False)
            SetAsPTX(TX_ADDR)   'Set the nRF24L01p as a transmitter. Will pass the TX_ADDR as a momentary RX_ADDR_P0
            WritePayload(buffer)
            SetChipEnable(True)
            Task.Delay(TimeSpan.FromTicks(10000)).Wait()
            SetChipEnable(False)
            Task.Delay(TimeSpan.FromTicks(50000)).Wait() 'Délai pour la transmission
        Next

        SetAsPRX()
        SetChipEnable(True)

    End Sub

    Private Sub NordicInterruptPin_ValueChanged(sender As GpioPin, args As GpioPinValueChangedEventArgs) Handles NordicInterruptPin.ValueChanged
        If args.Edge = GpioPinEdge.FallingEdge Then

            If NordicInterruptPin.Read() = GpioPinValue.Low Then

                'Get the STATUS Register
                Dim writeBuffer(1) As Byte
                Dim StatusBuffer(1) As Byte

                writeBuffer(0) = Common.Commands.NOP
                NordicSPI.TransferFullDuplex(writeBuffer, StatusBuffer)

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

                NordicInterruptPin.Write(GpioPinValue.High)

            End If

        End If
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