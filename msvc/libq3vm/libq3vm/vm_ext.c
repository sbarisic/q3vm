#include <vm.h>

/** Translate from real machine memory to virtual machine memory
 * @param[in] Addr Address in real machine memory
 * @param[in,out] vm Current VM
 * @return translated address. */
intptr_t VM_ArgPtr2(void* Addr, vm_t* vm)
{
    if (vm == NULL)
    {
        Com_Error(VM_INVALID_POINTER, "Invalid VM pointer");
        return NULL;
    }

    intptr_t vmAddr = (intptr_t)Addr - (intptr_t)(vm->dataBase);

    if (VM_MemoryRangeValid(vmAddr, 0, vm) == 0)
        return vmAddr;

    return NULL;
}