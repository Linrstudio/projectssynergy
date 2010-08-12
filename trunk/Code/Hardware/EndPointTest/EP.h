#ifndef EP_H
#define EP_H

#include "Default.h"
#include "EP.h"

void Init();
void Tick();

void Polled();

int8 InvokeEvent(int8 _Event,int16 _Args);

#endif