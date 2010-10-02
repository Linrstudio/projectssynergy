
#include "Default.h"

void MemoryBeginWrite(int16 _Addr);
void MemoryWrite(int8 _Data);
void MemoryEndWrite();
void MemoryBeginRead(int16 _Addr);
int8 MemoryReadInt8();
void MemoryEndRead();

