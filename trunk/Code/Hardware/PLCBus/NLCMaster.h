#include "Default.h"

#define CLK GP5
#define DAT GP4
#define DATBIT 4

void NLCInit();
void NLCDelay();
void NLCWriteByte(int8 _Data);
int8 NLCReadByte();
void NLCWriteStart();