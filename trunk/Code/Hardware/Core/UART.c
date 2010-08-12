#include"Default.h"
#include"UART.h"

#include "Compiler.h"

#include "HardwareProfile.h"

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
	UART_DIR_TRIS	=0;			//output
	UART_DIR=0;					//read by default

	//bitrate = 10417
	BAUDCONbits.BRG16=0;
	TXSTAbits.BRGH=0;
	SPBRGH=0;
	SPBRG =255;
	
	//invert signals
#if 0
	BAUDCONbits.CKTXP=1;
	BAUDCONbits.DTRXP=1;
#endif

	UART_DIR_TRIS=0;
}

int8 UARTReadInt8(void)
{
	while(!PIR1bits.RCIF)
	{
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

int8 UARTAvailable(void)
{
  if (PIR1bits.RCIF) return 255;
  return 0;
}

void UARTWrite()
{
	int i;
	int j;
	UART_DIR=1;
	for(i=0;i<255;i++);
	TXSTAbits.TXEN=0;
	TXSTAbits.TXEN=1;
	for(i=0;i<255;i++)for(i=0;i<255;i++);
}

void UARTRead()
{
	int i=0;
	for(i=0;i<255;i++);
	UART_DIR=0;
	for(i=0;i<255;i++);
	RCSTAbits.CREN=0;
	RCSTAbits.CREN=1;
	for(i=0;i<255;i++);
}

void UARTWriteInt8(int8 c)
{
	int i;
	while(!PIR1bits.TXIF)UARTClearErrors;
	TXREG=c;
	while(!PIR1bits.TXIF)UARTClearErrors;
	for(i=0;i<255;i++);
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