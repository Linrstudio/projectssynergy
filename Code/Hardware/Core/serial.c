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

#define UARTClearErrors\
{\
if (OERR){TXEN=0; TXEN=1; CREN=0; CREN=1;}\
if (FERR){TXEN=0; TXEN=1;}\
}

unsigned char UARTRead(void)
{
	while(!RCIF)
	{
		CLRWDT();
		UARTClearErrors;
	}
	return RCREG;
}

unsigned int UARTReadBool(void)
{
	return UARTRead()!='0'?1:0;
}

unsigned char UARTAvailable(void)
{
  if (RCIF) return 1;
  return 0;
}

void UARTWrite(unsigned char c)
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
		putch(*str);
    if (*str==13) putch(10);
    if (*str==10) putch(13);
		str++;
	}
}

