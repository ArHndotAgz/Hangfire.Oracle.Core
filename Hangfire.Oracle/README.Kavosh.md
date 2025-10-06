# Kavosh.Hangfire.Oracle.Core

A customized fork of Hangfire.Oracle.Core with support for **dynamic table naming** and **configurable Oracle data types**.

## Why This Fork?

This fork was created to address enterprise requirements where:
- Database table names must follow specific naming conventions
- Schema names need to be configurable
- Oracle data types (NVARCHAR2 vs VARCHAR2, NCLOB vs CLOB) need to be controllable
- Tables must be created with company-specific prefixes

## Key Features

✅ **Dynamic Table Naming** - Configure custom table names via JSON or code  
✅ **Schema Configuration** - Set Oracle schema via environment variables or config  
✅ **Data Type Control** - Choose between NVARCHAR2/VARCHAR2 and NCLOB/CLOB  
✅ **Backward Compatible** - Works with default Hangfire table names if not configured  
✅ **Enterprise Ready** - Supports company naming conventions and standards  

## Installation
