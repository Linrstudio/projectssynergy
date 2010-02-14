#ifndef MEMORY_H
#define MEMORY_H
#include "Default.h"

//for now we use the internal EEPROM even though it is only 256 byte, we use 16bits addresses

#define MEMORYSIZE 1024

#define MEMORYEXTERNAL

#define MEMORYRXDIR  	TRISC1
#define MEMORYTXDIR  	TRISC2
#define MEMORYCLKDIR 	TRISC0
#define MEMORYCSDIR 	TRISB4

#define MEMORYRX  		RC1
#define MEMORYTX  		RC2
#define MEMORYCLK 		RC0
#define MEMORYCS 		RB4

extern void MemoryInit();
extern void MemoryWait();
extern int8 MemoryReadInt8(int16 _Addr);
extern int16 MemoryReadInt16(int16 _Addr);
extern void MemoryWriteInt8(int16 _Addr,int8 _Data);

extern void MemorySPIDelay();
extern void MemorySPIWrite(int16 _Addr,int8 _Data);
extern int8 MemorySPIRead(int16 _Addr);
extern void MemorySPIWriteRaw(int8 _Data);
extern int8 MemorySPIReadRaw();
extern void MemorySPIWriteEnable();
extern void MemorySPIWriteDisable();

extern int8 MemorySPIReadStatus();

#endif