#ifndef KISMET_H
#define KISMET_H
#include "Default.h"

int16 KismetRegisters[32];
int16 KismetVariables[64];

int8 KismetEnabled;

extern void KismetInit();

extern void KismetSetRegister(int8 _Register,int16 _Value);
extern int16 KismetGetRegister(int8 _Register);

extern void KismetExecuteMethod(int16 _Deviceid,int16 _MethodAddr);
extern void KismetExecuteEvent(int16 _DeviceID,int8 _EventID,int16 _EventArgs);
#endif