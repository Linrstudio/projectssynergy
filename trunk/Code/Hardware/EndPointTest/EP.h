#ifndef EP_H
#define EP_H

#include "Default.h"

void EPInit();
void EPTick();
int8 EPGetType();
void EPPolled();

int8 EPInvokeEvent(int8 _Event,int8* _Args);

#endif