#ifndef _STDARG_H
#define _STDARG_H

#define __va_list char*
#define va_list __va_list

#define _INTSIZEOF(n) ((sizeof(n) + sizeof(int) - 1) & ~(sizeof(int) - 1))
#define _ADDRESSOF(v) (&(v))

#define __crt_va_start_a(ap, v)                                                \
    ((void)(ap = (va_list)_ADDRESSOF(v) + _INTSIZEOF(v)))
#define __crt_va_arg(ap, t) (*(t*)((ap += _INTSIZEOF(t)) - _INTSIZEOF(t)))
#define __crt_va_end(ap) ((void)(ap = (va_list)0))

#define va_start __crt_va_start_a
#define va_arg __crt_va_arg
#define va_end __crt_va_end
#define va_copy(destination, source) ((destination) = (source))

#endif
