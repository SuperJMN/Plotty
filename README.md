# Plotty
## Emulator of a custom RISC architecture using [Superpower](https://github.com/nblumhardt/superpower)

This is **Plotty**, a side-project just for The Fun Of Learning®. 

# Based on [Superpower](https://github.com/nblumhardt/superpower)
## A very interesting and powerful library by [Nicholas Blumhardt](https://github.com/nblumhardt) to build parsers easily in .NET.

To summarize, this project goes around a custom RISC architecture I created. It can be programmed by writing in an assembly language than is then parsed to instructions and executed by an interpreter. 

The cool and interesting thing about it is the parser that takes the assembly code and transforms it into instructions. If you want to learn about parsing with **Superpower**, you can use this for reference to create your own parsers :)

# Do you want to see it running? 
Then, watch [this video](https://files.gitter.im/datalust/superpower/yLt8/2018-03-22_18-50-20.mp4) :) 

# Example code:
```
	MOVE	R0,#27
	MOVE	R1,#65
start:	STORE	R1,0+R2
	ADD	R1,#1
	ADD	R2,#1
	BRANCH	R0,R2,end
	BRANCH	R3,R3,start
end:	HALT
```

# Reference
- **MOVE** destination, source: Moves values between registers. The source argument can be a number
- **STORE** source,destination: Stores a value in memory. 
- **ADD** source, value[, destination]: Adds a value. If the destination isn't specified, the source will be the destination (implicit destination)
- **BRANCH** register1, register2, target: Compares the values of register1 and register2. If both are equal, the execution jumps to the specified target (label or line)
- **HALT**: halts the execution.

## NOTICE: 
- Immediate values are prefixed with **#** (hash)
- The first 80x80 bytes of memory are mapped to the **Console**, so writing a value of 65 (ASCII code for 'A') to the address 0 will result in a letter A at the top-left part of the console (if you're running the Plotty.Uwp emulator).
