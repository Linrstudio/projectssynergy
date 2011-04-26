#include "MainStation.h"
#include "EP.h"
#include "RTC.h"
#include "UART.h"
#include "I2C.h"
#include "Default.h"
#include "Kismet.h"

int8 OperationEnabled=0;

void MSUpdate()
{
	EPUpdate();
	RTCUpdate();	
	USBUpdate();
}

void SetLED(int8 _State)
{
	LED=(_State!=0)?1:0;
}

void DisableOperation()
{
	OperationEnabled=0;
}

void EnableOperation()
{
	OperationEnabled=255;
	KismetExecuteEvent(0,1);
}

void MSInit()
{
	//turn off A lot of crap
	CM1CON0bits.C1ON=0;
	CM1CON0=0;
	CM2CON0=0;
	SRCON0bits.SRLEN=0;
	//disable analog
	ANSEL=0;
	ANSELH=0;
	ADCON0bits.ADON=0;
	
	UARTInit();
	RTCInit();
	EPInit();
	USBInit();
	I2CInit();

	LED_LATCH=0;
	LED_TRIS=0;

	SetLED(0);
	EnableOperation();
}
