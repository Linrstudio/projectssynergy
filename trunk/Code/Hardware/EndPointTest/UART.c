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

	BRG16 =0;
	BRGH  =0;
	SPBRGH=0;
	SPBRG =64;

	//invert signals
#if 0
	CKTXP=1;
	DTRXP=1;
#endif
	
	UART_DIR_TRIS=0;
	UART_DIR=0;

	TRISB5=1;//RX
	TRISB7=0;//TX
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
  if (RCIF) return 1;
  return 0;
}

void UARTWrite()
{
	int i,j,k;
	UART_DIR=1;
	CREN=0;//disable RX
	TXEN=0;
	TXEN=1;//enable  TX
	for(i=0;i<255;i++)for(j=0;j<12;j++);//for(k=0;k<2;k++);
}

void UARTRead()
{
	int i;
	UART_DIR=0;
	RCIF=0;
	TXEN=0;//disable TX
	CREN=0;
	CREN=1;//enable  RX
	for(i=0;i<255;i++);
}

void UARTWriteInt8(int8 c)
{
	while(!TXIF)UARTClearErrors;
	TXREG=c;
	for(int i=0;i<255;i++);
	while(!TXIF)UARTClearErrors;
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