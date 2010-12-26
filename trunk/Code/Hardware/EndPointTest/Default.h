#ifndef DEFAULT_H
#define DEFAULT_H

#include "Compiler.h"

typedef unsigned char int8;
typedef unsigned short int16;
#define INT16(x)(*((int16*)&x))
#define INT8(x)(*((int8*)&x))
#endif