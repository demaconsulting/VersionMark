# Off-The-Shelf Component Verification

## Overview

This section documents the verification evidence for each Off-The-Shelf (OTS) component
used by VersionMark. OTS components are third-party tools and libraries that are not
developed in-house. Their verification relies on the vendor's own quality assurance and,
where available, self-validation mechanisms provided by the tool itself.

For OTS tools that include a `--validate` flag, VersionMark's CI pipeline runs
self-validation and captures the results as TRX artifacts. For tools without a built-in
self-validation mode, verification is based on functional evidence produced during the
build and document generation pipeline.

The following OTS components are covered in this section:

- **BuildMark** - build notes generation tool
- **FileAssert** - file content assertion tool
- **MSTest** - unit testing framework
- **Pandoc** - document conversion tool
- **ReqStream** - requirements traceability tool
- **ReviewMark** - code review enforcement tool
- **SarifMark** - SARIF report tool
- **SonarMark** - SonarCloud report tool
- **WeasyPrint** - HTML-to-PDF conversion tool
