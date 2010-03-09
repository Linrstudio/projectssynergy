#ifndef MEMORY_H
#define MEMORY_H
#include "Default.h"

//for now we use the internal EEPROM even though it is only 256 byte, we use 16bits addresses
#define MEMORYRXDIR  	TRISC1
#define MEMORYTXDIR  	TRISC2
#define MEMORYCLKDIR 	TRISB4
#define MEMORYCSDIR 	TRISC0

#define MEMORYRX  		RC1
#define MEMORYTX  		RC2
#define MEMORYCLK 		RB4
#define MEMORYCS 		RC0

extern void MemoryInit();
extern void MemoryWriteInt8(int16 _Addr,int8 _Data);
extern int8 MemoryReadInt8(int16 _Addr);
extern int16 MemoryReadInt16(int16 _Addr);

extern void MemoryWriteEnable();
extern void MemoryWriteDisable();

extern void MemorySPIDelay();
extern void MemorySPIWriteRaw(int8 _Data);
extern int8 MemorySPIReadRaw();

extern int8 MemoryReadStatus();

#endif