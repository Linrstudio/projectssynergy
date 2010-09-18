#include "EP.h"

//Init method for the program
void Init()
{

}

//main Tick method for the program
void Tick()
{
	
}

//when polled by the main station
void Polled()
{
	RC7=!RC7;
}

//request to invoke event
//returns wether the event was successfully invoked
int8 InvokeEvent(int8 _Event,int16 _Args)
{
	RC2=!RC2;
/*
	switch(_Event)
	{
		case 1:
			RC2=0;
		break;
		case 2:
			RC2=1;
		break;
	}
*/
}