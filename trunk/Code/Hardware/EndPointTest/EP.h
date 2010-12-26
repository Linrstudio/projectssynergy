#ifndef EP_H
#define EP_H

#include "Default.h"
#include "EP.h"

void EPInit();
void EPTick();

void EPPolled();

int8 EPInvokeEvent(int8 _Event,int8* _Args);

#endif