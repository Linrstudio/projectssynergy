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
}

unsigned char UARTReadInt8(void)
{
	while(!RCIF)
	{
		CLRWDT();
		UARTClearErrors;
	}
	return RCREG;
}

unsigned short UARTReadInt16()
{
	short bob;
	char*dat=&bob;
	dat[1]=UARTReadInt8();
	dat[0]=UARTReadInt8();
	return bob;
}

unsigned int UARTReadBool(void)
{
	return UARTReadInt8()!='0'?1:0;
}

unsigned char UARTAvailable(void)
{
  if (RCIF) return 1;
  return 0;
}

void UARTWriteInt8(int8 c)
{
	while(!TXIF)			//set when register is empty
	{
		UARTClearErrors;
		CLRWDT();
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
    unsigned short bob=_Value;
	int8*dat = &bob;
	UARTWriteInt8(dat[1]);
	UARTWriteInt8(dat[0]);
}