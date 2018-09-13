#ifndef QVM_API_H
#define QVM_API_H

#ifndef API_EXTERN
#define API_EXTERN extern
#endif

API_EXTERN int __args[11];
API_EXTERN int __cmd;

#include <stdint.h>
#include <api_generated.h>
#include "printf.h"

int main();

#endif
