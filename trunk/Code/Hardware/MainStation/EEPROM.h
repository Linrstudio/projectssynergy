#ifndef EEPROM_H
#define EEPROM_H

#include "Default.h"

void EEPROMWrite(int8 _AddrHi,int8 _AddrLo,int8 _Data);
void EEPROMBeginRead(int8 _AddrHi,int8 _AddrLo);
int8 EEPROMRead();
void EEPROMEndRead();

#endif