#include"Default.h"
#include"UART.h"

#include "Compiler.h"

#include "HardwareProfile.h"

#define TIMER

void UARTInit()
{
	TXSTAbits.SYNC=0;						//asynchronous
	RCSTAbits.SPEN=1;						//enable serial port pins
	RCSTAbits.CREN=1;						//enable reception
	RCSTAbits.SREN=0;						//no effect
	TXSTAbits.TX9=0;						//8-bit transmission
	RCSTAbits.RX9=0;						//8-bit reception
	TXSTAbits.TXEN=0;						//reset transmitter
	TXSTAbits.TXEN=1;						//enable the transmitter

	UART_RX_TRIS	=1;			//input
	UART_TX_TRIS	=0;			//output
	UART_DIR_TRIS	=1;			//output
	UART_DIR=0;					//read by default

	//bitrate = 10417
	BAUDCONbits.BRG16=1;
	TXSTAbits.BRGH=0;

	SPBRGH=4;
	SPBRG =0;
	
	//invert signals
#if 0
	BAUDCONbits.CKTXP=1;
	BAUDCONbits.DTRXP=1;
#endif

	UART_DIR_TRIS=0;


	//enable timer
	T0CONbits.PSA=0;
	T0CONbits.T0PS0=1;
	T0CONbits.T0PS1=1;
	T0CONbits.T0PS2=1;
	T0CONbits.T08BIT=1;
	T0CONbits.TMR0ON=1;
	T0CONbits.T0CS=0;
}

int8 UARTReadInt8(void)
{
#ifdef TIMER
	TMR0L=0;
	INTCONbits.TMR0IF=0;
#endif
	while(!PIR1bits.RCIF)
	{
#ifdef TIMER
		if(INTCONbits.TMR0IF)
		{
			UARTError=255;
			return 0;
		}
#endif
		UARTClearErrors;
	}
	return RCREG;
}

int16 UARTReadInt16()
{
	short var;
	char*dat=&var;
	dat[1]=UARTReadInt8();
	dat[0]=UARTReadInt8();
	return var;
}

int8 UARTAvailable()
{
  if (PIR1bits.RCIF) return 255;
  return 0;
}

void UARTWrite()
{
	//give clients a chance to go in read mode
	TMR0L=0;
	INTCONbits.TMR0IF=0;
	while(!INTCONbits.TMR0IF);

	UART_DIR=1;
	PIR1bits.TXIF=0;
	TXSTAbits.TXEN=0;
	TXSTAbits.TXEN=1;
}

void UARTRead()
{
	while(!TXSTAbits.TRMT);//wait for last write to complete
	UART_DIR=0;
	PIR1bits.RCIF=0;
	RCSTAbits.CREN=0;
	RCSTAbits.CREN=1;
	RCREG=RCREG;
}

void UARTWriteInt8(int8 c)
{
	int i;
	while(!PIR1bits.TXIF)UARTClearErrors;
	TXREG=c;
	while(!PIR1bits.TXIF)UARTClearErrors;
}

void UARTWriteString(register const char *str)
{
	while((*str)!=0)
	{
	 	UARTWriteInt8(*str);
		str++;
	}
}

void UARTWriteInt16(int16 _Value)
{
	int8*dat = &_Value;
	UARTWriteInt8(dat[1]);
	UARTWriteInt8(dat[0]);
}