#include <api.h>

int main()
{
    int   Value    = 420;
    void* PtrValue = &Value;

    printf("Hello QVM World! %i\n", __cmd);

    printf("PtrValue = %i\n", PtrValue);
    printf("Test(PtrValue) = %i\n", Test(PtrValue));

    return __cmd;
}
