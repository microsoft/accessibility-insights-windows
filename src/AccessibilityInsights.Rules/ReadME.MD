Before you create or modify any rules, please read this document.

##Creation of rules
1. Rules are created based on  standards like MSDN and WCAG
2. Rules may be created based on existing accessibility tools after a review by Microsoft.
3. Rules that don't overlap with standards-based rules may be added after a review by Microsoft.

##Rule modification
1. Rules will be modified when standard guidelines or MSDN change. 
2. Rules that don't overlap with standards-based rules may be modified after a review by Microsoft.

##Severity level of a scanned error
1. All errors , detected by Standard based rules, are reported as an error by default. 
2. If there is a false positive reported by a customer, after a review by Microsoft, the severity level may be adjusted to warning. 

##For this specific case, let's create 4 different rules for button pattern.
- Invoke and Toggle can't exist together (if not, Error)
- Invoke and ExpandCollapse may exist but not recommended. (if so, warning)
- Toggle and ExpandCollapse can't exist together (if not, Error)

##Caveats
Please don't remove or rename the RuleId(s). It will break backward compatibility since we serialize ruleIDs in string. 