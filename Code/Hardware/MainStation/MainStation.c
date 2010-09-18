#include "MainStation.h"
#include "EP.h"
#include "RTC.h"
#include "UART.h"

#include "Default.h"


void MSInit()
{
	UARTInit();
	RTCInit();
	EPInit();
	USBInit();
}

void MSUpdate()
{
	RTCUpdate();
	EPUpdate();
	USBUpdate();
}
