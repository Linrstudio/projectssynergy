#include"Default.h"
#include"UART.h"

void UARTInit()
{
	SYNC=0;						//asynchronous
	SPEN=1;						//enable serial port pins
	CREN=1;						//enable reception
	SREN=0;						//no effect4
	TX9=0;						//8-bit transmission
	RX9=0;						//8-bit reception
	TXEN=0;						//reset transmitter
	TXEN=1;						//enable the transmitter
	
	BRG16 =1;
	BRGH  =0;

	SPBRGH=1;
	SPBRG =0;

	//invert signals
#if 0
	CKTXP=1;
	DTRXP=1;
#endif
	
	UART_DIR_TRIS=0;
	UART_DIR=0;

	TRISB5=1;//RX
	TRISB7=0;//TX


	//enable timer
	PSA=0;
	PS0=1;
	PS1=1;
	PS2=0;

	T0CS=0;
}

int8 UARTReadInt8(void)
{
	while(!RCIF)
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

int8 UARTReadBool(void)
{
	return UARTReadInt8()!='0'?1:0;
}

int8 UARTAvailable(void)
{
  if (RCIF) return 255;
  return 0;
}

void UARTWrite()
{
	TMR0=0;
	T0IF=0;
	while(!T0IF);

	UART_DIR=1;
	TXEN=0;
	TXEN=1;//enable  TX
}

void UARTRead()
{
	while(!TRMT);

	UART_DIR=0;
	RCIF=0;
	CREN=0;
	CREN=1;//enable  RX
}

void UARTWriteInt8(int8 c)
{
	while(!TXIF)UARTClearErrors;
	TXREG=c;
	while(!TXIF)UARTClearErrors;
}

void UARTWriteInt16(int16 _Value)
{
	int8*dat = &_Value;
	UARTWriteInt8(dat[1]);
	UARTWriteInt8(dat[0]);
}