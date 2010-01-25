#include <pic18.h>

typedef unsigned short int16;
typedef unsigned char int8;

#define COMPAREINT16(_A,_B) (((int8*)&_A)[0]==((int8*)&_B)[0]&&((int8*)&_A)[1]==((int8*)&_B)[1])
#define COMPAREINT8 (int8  _A,int8  _B) (_A==_B)
