This is the current documentation about the structure of BEA archives I have been able to wrote.

### BEA Header:
| Offset | Size | Description |
|---|---|---|
| 0x00 | 0x4 u32 | Magic (SCNE)
| 0x04 | 0x4 u32 | Reserved ? Always 0x00
| 0x08 | 0x4 u32 | Unknown ? Always 0x10000
| 0x0C | 0x2 u16 | Byte Order Mark (BOM): 0xFEFF for big endian and 0xFFFE for little endian
| 0x0E | 0x2 u16 | Section number ? Always 0x4
| 0x10 | 0x4 u32 | Reserved ? Always 0x00
| 0x12 | 0x2 u16 | Reserved ? Always 0x00
| 0x14 | 0x2 u16 | First ASST section offset
| 0x18 | 0x4 u32 | _RLT section offset
| 0x1C | 0x4 u32 | All sections size (this size need to be align up to 0x10 to get file data)
| 0x20 | 0x8 u64 | Files number
| 0x28 | 0x8 u64 | ASST sections info offset - Always 0x48 ?
| 0x30 | 0x8 u64 | _DIC section offset
| 0x38 | 0x8 u64 | Unknown ? Always 0x00
| 0x40 | 0x8 u64 | Archive name offset

### ASST sections info: 
| Offset | Size | Description |
|---|---|---|
| 0x48 | 0x8 u64 | First ASST section offset
| 0x50 | 0x8 u64 | Second ASST section offset
| etc...| 0x8 u64 | each 0x8 * Files number

### Filenames:
| Offset | Size | Description |
|---|---|---|
| 0x00 | 0x2 u16 | Filename size
| 0x02 | Size    | Filename
| Size | 0x2 u16 | Reserved ? Always 0x00

### ASST Section:
| Offset | Size | Description |
|---|---|---|
| 0x00 | 0x4 u32 | Magic (ASST)
| 0x04 | 0x4 u32 | Section Size ? Always 0x30
| 0x08 | 0x4 u32 | Section Size ? Always 0x30
| 0x0C | 0x4 u32 | Reserved
| 0x10 | 0x2 u16 | Compression Type ?
| 0x12 | 0x2 u16 | Alignment size
| 0x14 | 0x4 u32 | File size compressed
| 0x18 | 0x4 u32 | File size decompressed
| 0x1C | 0x4 u32 | Reserved
| 0x20 | 0x8 u64 | File offset
| 0x28 | 0x8 u64 | Filename offset

