#define API_EXTERN
#include "api.h"

int vmMain(int command, int arg0, int arg1, int arg2, int arg3, int arg4,
           int arg5, int arg6, int arg7, int arg8, int arg9, int arg10,
           int arg11)
{
    __cmd      = command;
    __args[0]  = arg0;
    __args[1]  = arg1;
    __args[2]  = arg2;
    __args[3]  = arg3;
    __args[4]  = arg4;
    __args[5]  = arg5;
    __args[6]  = arg6;
    __args[7]  = arg7;
    __args[8]  = arg8;
    __args[9]  = arg9;
    __args[10] = arg10;
    __args[11] = arg11;

    return main();
}
