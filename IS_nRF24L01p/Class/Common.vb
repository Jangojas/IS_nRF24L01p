Public NotInheritable Class Common

#Region "Commands"

    ''' <summary>
    '''   Commands for <see cref="nRF24L01p"/>
    ''' </summary>
    Public Enum Commands As Byte

        ''' <summary>
        ''' Read command and status registers. AAAAA = 5 bit Register Map Address
        ''' </summary>
        R_REGISTER = &H0

        ''' <summary>
        ''' Write command and status registers. AAAAA = 5 bit Register Map Address Executable in power down or standby modes only.
        ''' </summary>
        W_REGISTER = &H20

        ''' <summary>
        ''' Read RX-payload: 1 – 32 bytes. A read operation always starts at byte 0. Payload is deleted from FIFO after it is read.
        ''' </summary>
        R_RX_PAYLOAD = &H61

        ''' <summary>
        ''' Write TX-payload: 1 – 32 bytes. A write operation always starts at byte 0 used in TX payload.
        ''' </summary>
        W_TX_PAYLOAD = &HA0

        ''' <summary>
        ''' Flush TX FIFO, used in TX mode
        ''' </summary>
        FLUSH_TX = &HE1

        ''' <summary>
        ''' Flush RX FIFO, used in RX mode Should not be executed during transmission of acknowledge, 
        ''' that is, acknowledge package will not be completed.
        ''' </summary>
        FLUSH_RX = &HE2

        ''' <summary>
        ''' Used for a PTX device Reuse last transmitted payload. TX payload reuse is active until
        ''' W_TX_PAYLOAD or FLUSH TX is executed. TX payload reuse must not be activated or 
        ''' deactivated during package transmission.
        ''' </summary>
        REUSE_TX_PL = &HE3

        ''' <summary>
        ''' Read RX payload width for the top R_RX_PAYLOADin the RX FIFO.
        ''' </summary>
        R_RX_PL_WID = &H60

        ''' <summary>
        ''' Used in RX mode.
        ''' Write Payload to be transmitted together with ACK packet on PIPE PPP. (PPP valid in the 
        ''' range from 000 to 101). Maximum three ACK packet payloads can be pending. Payloads with 
        ''' same PPP are handled using first in - first out principle. Write payload: 1– 32 bytes.
        ''' A write operation always starts at byte 0. </summary>
        W_ACK_PAYLOAD = &HA8

        ''' <summary>
        ''' Used in TX mode. Disables AUTOACK on this specific packet.
        ''' </summary>
        W_TX_PAYLOAD_NO_ACK = &HB0

        ''' <summary>
        ''' No Operation. Might be used to read the STATUS register
        ''' </summary>
        NOP = &HFF
    End Enum

#End Region

#Region "Options"

    ''' <summary>
    ''' Address width option for <see cref="nRF24L01p"/>
    ''' </summary>
    Public Enum AddressWidth As Byte
        ADR_3 = 3
        ADR_4 = 4
        ADR_5 = 5
    End Enum

    ''' <summary>
    ''' Power option for <see cref="nRF24L01p"/>
    ''' </summary>
    Public Enum dbPower As Byte
        PWR_minus_18db = 0
        PWR_minus_12db = 1
        PWR_minus_6db = 2
        PWR_0db = 3
    End Enum

    ''' <summary>
    ''' Speed option for <see cref="nRF24L01p"/>
    ''' </summary>
    Public Enum RFSpeed As Byte
        LOW_250 = 0
        MID_1000 = 1
        HIGH_2000 = 2
    End Enum

#End Region

#Region "Registers"

    ''' <summary>
    ''' Registers for <see cref="nRF24L01p"/>
    ''' </summary>
    Public Enum Registers As Byte
        CONFIG = &H0
        EN_AA = &H1
        EN_RXADDR = &H2
        SETUP_AW = &H3
        SETUP_RETR = &H4
        RF_CH = &H5
        RF_SETUP = &H6
        STATUS = &H7
        OBSERVE_TX = &H8
        RPD = &H9
        RX_ADDR_P0 = &HA
        RX_ADDR_P1 = &HB
        RX_ADDR_P2 = &HC
        RX_ADDR_P3 = &HD
        RX_ADDR_P4 = &HE
        RX_ADDR_P5 = &HF
        TX_ADDR = &H10
        RX_PW_P0 = &H11
        RX_PW_P1 = &H12
        RX_PW_P2 = &H13
        RX_PW_P3 = &H14
        RX_PW_P4 = &H15
        RX_PW_P5 = &H16
        FIFO_STATUS = &H17
        DYNPD = &H1C
        FEATURE = &H1D
    End Enum

#End Region

#Region "BitFlags"

    Public Enum BitFlags As Byte

        ''' <summary>
        ''' Mask interrupt caused by RX_DR
        ''' 1: Interrupt not reflected on the IRQ pin
        ''' 0: Reflect RX_DR as active low interrupt on the IRQpin
        ''' </summary>
        MASK_RX_DR = 6

        ''' <summary>
        ''' Mask interrupt caused by TX_DS
        ''' 1: Interrupt not reflected on the IRQ pin
        ''' 0: Reflect TX_DSas active low interrupt on the IRQ pin
        ''' </summary>
        MASK_TX_DS = 5

        ''' <summary>
        ''' Mask interrupt caused by MAX_RT
        ''' 1: Interrupt not reflected on the IRQ pin
        ''' 0: Reflect MAX_RTas active low interrupt on the IRQ pin
        ''' </summary>
        MASK_MAX_RT = 4

        ''' <summary>
        ''' Enable CRC. Forced high if one of the bits in the EN_AA is high
        ''' </summary>
        EN_CRC = 3

        ''' <summary>
        ''' CRC encoding scheme
        ''' '0' - 1 Byte
        ''' '1' – 2 Bytes
        ''' </summary>
        CRCO = 2

        ''' <summary>
        ''' 1: POWER UP, 0:POWER DOWN
        ''' </summary>
        PWR_UP = 1

        ''' <summary>
        ''' RX/TX control 
        ''' 1: PRX, 0: PTX 
        ''' </summary>
        PRIM_RX = 0

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 5
        ''' </summary>
        ENAA_P5 = 5

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 4
        ''' </summary>
        ENAA_P4 = 4

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 3
        ''' </summary>
        ENAA_P3 = 3

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 2
        ''' </summary>
        ENAA_P2 = 2

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 1
        ''' </summary>
        ENAA_P1 = 1

        ''' <summary>
        ''' Enable auto acknowledgement data pipe 0
        ''' </summary>
        ENAA_P0 = 0

        ''' <summary>
        ''' Enable data pipe 5.
        ''' </summary>
        ERX_P5 = 5

        ''' <summary>
        ''' Enable data pipe 4.
        ''' </summary>
        ERX_P4 = 4

        ''' <summary>
        ''' Enable data pipe 3.
        ''' </summary>
        ERX_P3 = 3

        ''' <summary>
        ''' Enable data pipe 2.
        ''' </summary>
        ERX_P2 = 2

        ''' <summary>
        ''' Enable data pipe 1.
        ''' </summary>
        ERX_P1 = 1

        ''' <summary>
        ''' Enable data pipe 0.
        ''' </summary>
        ERX_P0 = 0

        ''' <summary>
        ''' RX/TX Address field width 
        ''' '00' - Illegal
        ''' '01' - 3 Bytes 
        ''' '10' - 4 Bytes 
        ''' '11' – 5 Bytes
        ''' </summary>
        AW_LOW = 0

        ''' <summary>
        ''' RX/TX Address field width 
        ''' '00' - Illegal
        ''' '01' - 3 Bytes 
        ''' '10' - 4 Bytes 
        ''' '11' – 5 Bytes
        ''' </summary>
        AW_HIGH = 1

        ''' <summary>
        ''' Auto Retransmit Delay
        ''' </summary>
        ARD = 4

        ''' <summary>
        ''' Auto Retransmit Count
        ''' </summary>
        ARC = 0

        ''' <summary>
        ''' Enables continuous carrier transmit when high
        ''' </summary>
        CONT_WAVE = 7

        ''' <summary>
        ''' 
        ''' </summary>
        RF_DR_LOW = 5

        ''' <summary>
        ''' Force PLL lock signal. Only used in test
        ''' </summary>
        PLL_LOCK = 4

        ''' <summary>
        ''' 
        ''' </summary>
        RF_DR_HIGH = 3

        ''' <summary>
        ''' Set RF output power in TX mode Low Bit
        ''' '00' – -18dBm
        ''' '01' – -12dBm
        ''' '10' – -6dBm
        ''' '11' – 0dBm
        ''' </summary>
        RF_PWR_LOW = 1

        ''' <summary>
        ''' Set RF output power in TX mode High Bit
        ''' '00' – -18dBm
        ''' '01' – -12dBm
        ''' '10' – -6dBm
        ''' '11' – 0dBm
        ''' </summary>
        RF_PWR_HIGH = 2

        ''' <summary>
        ''' Data Ready RX FIFO interrupt. Asserted when new data arrives RX FIFO
        ''' </summary>
        RX_DR = 6

        ''' <summary>
        ''' Data Sent TX FIFO interrupt. Asserted when packet transmitted on TX. 
        ''' If AUTO_ACKis activated, this bit is set high only when ACK is received.
        ''' </summary>
        TX_DS = 5

        ''' <summary>
        ''' Maximum number of TX retransmits interrupt Write 1 to clear bit. 
        ''' If MAX_RTis asserted it must be cleared to enable further communication.
        ''' </summary>
        MAX_RT = 4

        ''' <summary>
        ''' Data pipe number for the payload available for reading from RX_FIFO
        ''' 000-101: Data Pipe Number
        ''' 110: Not Used
        ''' 111: RX FIFO Empty
        ''' </summary>
        RX_P_NO = 1

        ''' <summary>
        ''' TX FIFO full flag. 
        ''' 1: TX FIFO full. 
        ''' 0: Available locations in TX FIFO.
        ''' </summary>
        TX_FULL = 0

        ''' <summary>
        ''' Count lost packets. The counter is overflow protected to 15, and discontinues at max until reset. 
        ''' The counter is reset by writing to RF_CH.
        ''' </summary>
        PLOS_CNT = 4
        ''' <summary>
        ''' Count retransmitted packets. 
        ''' The counter is reset when transmission of a new packet starts.
        ''' </summary>
        ARC_CNT = 0

        ''' <summary>
        ''' Received Power Detector. This register is called CD (Carrier Detect) in the nRF24L01. 
        ''' The name is different in nRF24L01+ due to the different input power level threshold for this bit. 
        ''' </summary>
        RPD = 0
        ''' <summary>
        ''' Used for a PTX device Pulse the rfce high for at least 10µs to Reuse last transmitted payload.
        ''' TX payload reuse is active until W_TX_PAYLOADor FLUSH TXis executed.
        ''' TX_REUSE is set by the SPI command REUSE_TX_PL, and is reset by the SPI commands W_TX_PAYLOAD or FLUSH TX
        ''' </summary>
        TX_REUSE = 6
        ''' <summary>
        ''' TX FIFO full flag. 1: TX FIFO full. 0: Available locations in TX FIFO.
        ''' </summary>
        FIFO_FULL = 5
        ''' <summary>
        ''' TX FIFO empty flag. 
        ''' 1: TX FIFO empty. 
        ''' 0: Data in TX FIFO.
        ''' </summary>
        TX_EMPTY = 4
        ''' <summary>
        ''' RX FIFO full flag. 
        ''' 1: RX FIFO full. 
        ''' 0: Available locations in RX FIFO.
        ''' </summary>
        RX_FULL = 1
        ''' <summary>
        ''' RX FIFO empty flag. 
        ''' 1: RX FIFO empty. 
        ''' 0: Data in RX FIFO.
        ''' </summary>
        RX_EMPTY = 0
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P5 = 5
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P4 = 4
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P3 = 3
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P2 = 2
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P1 = 1
        ''' <summary>
        ''' Enable dynamic payload length data pipe 0.
        ''' </summary>
        DPL_P0 = 0
        ''' <summary>
        ''' Enables Dynamic Payload Length
        ''' </summary>
        EN_DPL = 2
        ''' <summary>
        ''' Enables Payload with ACK
        ''' </summary>
        EN_ACK_PAY = 1
        ''' <summary>
        ''' Enables the W_TX_PAYLOAD_NOACK command
        ''' </summary>
        EN_DYN_ACK = 0


    End Enum

#End Region



End Class