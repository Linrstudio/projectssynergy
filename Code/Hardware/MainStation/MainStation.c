#include "MainStation.h"
#include "EP.h"
#include "RTC.h"
#include "UART.h"
#include "I2C.h"
#include "Default.h"
#include "Kismet.h"

//shared memory layout:
//0-32 = kismet registers
int8 SharedMemory[EPBUFFERSIZE+(KISMETBUFFERSIZE*2)];

int8 OperationEnabled=255;

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

	LED1A_LATCH=0;
	LED1B_LATCH=0;
	LED2A_LATCH=0;
	LED2B_LATCH=0;

	LED1A_TRIS=0;
	LED1B_TRIS=0;
	LED2A_TRIS=0;
	LED2B_TRIS=0;

	SetLED1(0);
}

void MSUpdate()
{
	RTCUpdate();
	EPUpdate();
	USBUpdate();
}

void SetLED1(int8 _State)
{
	LED1B=(_State&1)!=0?0:1;
}

int8 GetLED1()
{
	return (LED1A?1:0);
}
