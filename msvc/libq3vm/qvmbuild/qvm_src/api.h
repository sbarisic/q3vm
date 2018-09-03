#ifndef QVM_API_H
#define QVM_API_H

#ifndef API_EXTERN
#define API_EXTERN extern
#endif

API_EXTERN int __args[11];
API_EXTERN int __cmd;

void __print(const char* str);
void __error(const char* str);

int main();

#endif
