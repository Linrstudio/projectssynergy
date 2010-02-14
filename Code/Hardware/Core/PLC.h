#ifndef PLC_H
#define PLC_H

#include <pic18.h>
#include "Default.h"

#define PLCRXDIR TRISC0
#define PLCTXDIR TRISC1
#define PLCCLKDIR TRISC2

#define PLCRX  RC0
#define PLCTX  RC1
#define PLCCLK RC2

int8 PLCReadBuffer;
int8 PLCReadIndex=8;
int8 PLCWriteBuffer;
int8 PLCWriteIndex=8;

extern void PLCInit();
extern int8 PLCReadInt8(void);
extern int8 PLCAvailable(void);
extern void PLCWriteInt8(int8 c);
extern void PLCDelay();
extern void PLCUpdate();
#endif