#include"Default.h"
#include"UART.h"


#define TIMER

void UARTRead()
{
	while(!TRMT);//wait for last write to complete
	UART_DIR_TRIS=0;
	UART_DIR=0;
	RCIF=0;
	CREN=0;
	CREN=1;
	RCREG=RCREG;
}

void UARTInit()
{
	SYNC=0;						//asynchronous
	SPEN=1;						//enable serial port pins
	CREN=1;						//enable reception
	SREN=0;						//no effect
	TX9=0;						//8-bit transmission
	RX9=0;						//8-bit reception
	TXEN=0;						//reset transmitter
	TXEN=1;						//enable the transmitter

	//LATBbits.LATB5=0;
	UART_RX_TRIS	=1;			//input
	UART_TX_TRIS	=0;			//output
	UART_DIR_TRIS	=0;			//output
	UART_DIR=0;					//read by default

	BRG16=1;
	BRGH=0;

	SPBRGH=1;
	SPBRG =0;

	TXIE=0;
	RCIE=0;

	//enable timer
	PSA=0;
	T0PS0=1;
	T0PS1=0;
	T0PS2=1;
	T08BIT=1;
	TMR0ON=1;
	T0CS=0;
	TMR0IE=0;

	UARTRead();
}

int8 UARTReadInt8(void)
{
	if (OERR)
	{
		CREN = 0;
		CREN = 1;
	}
	
#ifdef TIMER
	TMR0L=0;
	TMR0IF=0;
#endif
	while(!RCIF)
	{
#ifdef TIMER
		if(TMR0IF)
		{
			UARTError=255;
			return 0;
		}
#endif
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
	if (RCIF)return 255;
	return 0;
}

void UARTWrite()
{
	//give clients a chance to go in read mode
	TMR0L=0;
	TMR0IF=0;
	while(!TMR0IF);
	//UART_DIR_TRIS=0;
	UART_DIR=1;
	TXIF=0;
	TXEN=0;
	TXEN=1;
}

void UARTWriteInt8(int8 c)
{
	TMR0L=0;
	TMR0IF=0;
	while(!TXIF)
	{
		if(TMR0IF)
		{
			UARTError=255;
			return;
		}
	}
	TXREG=c;
}

void UARTWriteInt16(int16 _Value)
{
	int8*dat = &_Value;
	UARTWriteInt8(dat[1]);
	UARTWriteInt8(dat[0]);
}