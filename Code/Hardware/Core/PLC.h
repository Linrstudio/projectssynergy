#ifndef PLC_H
#define PLC_H

#include <pic18.h>
#include "Default.h"

#define PLCRXDIR  TRISC3
#define PLCTXDIR  TRISC4
#define PLCCLKDIR TRISC5

#define PLCRX  RC3
#define PLCTX  RC4
#define PLCCLK RC5

extern void PLCInit();
extern int8 PLCReadInt8();
extern int8 PLCAvailable();
extern void PLCWriteInt8(int8 c);
extern void PLCDelay();
extern void PLCUpdate();

extern void PLCWrite(int16 _Recipient,int16 _Data);
extern void PLCPoll(int16 _Recipient);

#endif