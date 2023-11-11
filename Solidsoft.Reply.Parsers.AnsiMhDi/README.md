This library provides a comprehensive validating parser for ANSI MH 10.8.2-2021 Data Identifiers (DIs).  See [MHI Publications | ANSI MH10.8.2 – Data Identifiers](https://my.mhi.org/s/store?_ga=2.238726597.458368394.1698339584-1963000240.1697407524#/store/browse/detail/a153h000005lJuRAAU).
The parser validates each DI against the format defined for that DI.  It calls back into an Action for each DI reported to it.  For each DI, parsed data is reported as a Resolved Entity object.  Each Resolved Entity includes a collection of all errors reported while parsing the DI.
The library depends on the Solidsoft.Reply.Parsers.Common library.

## Validation Errors:
For each invalid DI, the parser may output a combination of one or more errors.

The following validation errors are supported:

3001  
No data provided.

3002  
Invalid data identifier {0}.

3003  
Invalid format. No format {0} provided.

3004  
No records provided.

3005  
The value{0} is invalid for DI {1}.

3006  
The entity value cannot be null for DI{0}.

3007  
Validation for DI{0} timed out.

3008  
Invalid field. No data identifier.

3100
The value{0} does not match the specified pattern for the data element.
