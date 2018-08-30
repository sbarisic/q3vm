/*
      ___   _______     ____  __
     / _ \ |___ /\ \   / /  \/  |
    | | | |  |_ \ \ \ / /| |\/| |
    | |_| |____) | \ V / | |  | |
     \__\_______/   \_/  |_|  |_|


   Quake III Arena Virtual Machine

   Standalone interpreter: load a .qvm file, run it, exit.
*/

#include <stdio.h>
#include "vm.h"

#define EXPORT __declspec(dllexport)

typedef void (*VM_ErrorFunc)(vmErrorCode_t level, const char* error);
typedef intptr_t (*VM_MallocFunc)(size_t size, vm_t* vm, vmMallocType_t type);
typedef void (*VM_FreeFunc)(void* p, vm_t* vm, vmMallocType_t type);

static VM_ErrorFunc  OnError;
static VM_MallocFunc OnMalloc;
static VM_FreeFunc   OnFree;

int EXPORT VM_Init(VM_ErrorFunc onError, VM_MallocFunc onMalloc,
                   VM_FreeFunc onFree)
{
    OnError  = onError;
    OnMalloc = onMalloc;
    OnFree   = onFree;
    return sizeof(vm_t);
}

void Com_Error(vmErrorCode_t level, const char* error)
{
    OnError(level, error);
}

void* Com_malloc(size_t size, vm_t* vm, vmMallocType_t type)
{
    return OnMalloc(size, vm, type);
}

void Com_free(void* p, vm_t* vm, vmMallocType_t type)
{
    OnFree(p, vm, type);
}

/* The compiled bytecode calls native functions,
   defined in this file. */
intptr_t systemCalls(vm_t* vm, intptr_t* args);

int main(int argc, char** argv)
{
    vm_t vm;
    int  retVal = -1;

    if (argc < 2)
    {
        printf("No virtual machine supplied. Example: q3vm bytecode.qvm\n");
        return retVal;
    }

    char* filepath = argv[1];
    // uint8_t* image    = loadImage(filepath);
    uint8_t* image = NULL;

    if (!image)
    {
        return -1;
    }

    if (VM_Create(&vm, filepath, image, systemCalls) == 0)
    {
        retVal = VM_Call(&vm, 0);
    }

    VM_Free(&vm);
    free(image); /* we can release the memory now */

    return retVal;
}

/* Callback from the VM: system function call */
intptr_t systemCalls(vm_t* vm, intptr_t* args)
{
    int id = -1 - args[0];

    switch (id)
    {
    case -1: /* PRINTF */
        printf("%s", (const char*)VMA(1, vm));
        return 0;
    case -2: /* ERROR */
        fprintf(stderr, "%s", (const char*)VMA(1, vm));
        return 0;

    case -3: /* MEMSET */
        memset(VMA(1, vm), args[2], args[3]);
        return 0;

    case -4: /* MEMCPY */
        memcpy(VMA(1, vm), VMA(2, vm), args[3]);
        return 0;

    default:
        fprintf(stderr, "Bad system call: %ld", (long int)args[0]);
    }
    return 0;
}
