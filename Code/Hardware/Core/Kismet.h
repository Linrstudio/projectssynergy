#ifndef KISMET_H
#define KISMET_H
#include "Default.h"

int8 KismetRegisters[32];
int8 KismetVariables[256];

int8 KismetEnabled;

extern void KismetInit();

extern void KismetSetReg8(int8 _Register,int8 _Value);
extern int8 KismetGetReg8(int8 _Register);
extern void KismetSetReg16(int8 _Register,int16 _Value);

extern void KismetExecuteMethod(int16 _Deviceid,int16 _MethodAddr);
extern void KismetExecuteEvent(int16 _DeviceID,int8 _EventID,int8 _EventArgs);
#endif