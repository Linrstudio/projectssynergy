#include"Default.h"
#include"UART.h"

void UARTInit()
{
	//16Mhz, 1200 baud (note that the internal clock is 16Mhz)
	SPBRG=51;
	BRGH=1;
	BRG16=0;

	SYNC=0;						//asynchronous
	SPEN=1;						//enable serial port pins
	CREN=1;						//enable reception
	SREN=0;						//no effect
	TXIE=0;						//disable tx interrupts
	RCIE=0;						//disable rx interrupts
	TX9=0;						//8-bit transmission
	RX9=0;						//8-bit reception
	TXEN=0;						//reset transmitter
	TXEN=1;						//enable the transmitter

	//invert signals
#if 0
	CKTXP=1;
	DTRXP=1;
#endif
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

void UARTWriteInt8(int8 c)
{
	while(!TXIF)			//set when register is empty
	{
		UARTClearErrors;
	}
	TXREG=c;
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