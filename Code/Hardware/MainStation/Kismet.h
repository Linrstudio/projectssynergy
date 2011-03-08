#ifndef KISMET_H
#define KISMET_H
#include "Default.h"

#define KISMETBUFFERSIZE 32

extern unsigned char ToSendDataBuffer[64];

void KismetInit();

void Set8(int8 _Register,int8 _Value);
int8 Get8(int8 _Register);
void Set16(int8 _Register,int16 _Value);
int16 Get16(int8 _Register);

int8 KismetExecuteEvent(int16 _DeviceID,int8 _EventID);
#endif