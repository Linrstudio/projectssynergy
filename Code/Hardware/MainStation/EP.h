#ifndef EP_H
#define EP_H

#include "Default.h"

#define EEPROMHEADERSIZE 32
#define EPBUFFERSIZE 16

void EPInit();
void EPUpdate();
int8 EPSend(int16 _DeviceID);
int8 EPPoll(int16 _DeviceID);

#endif