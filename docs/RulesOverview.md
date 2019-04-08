## Rules Overview

Rules are the automated accessibility tests run by Accessibility Insights For Windows. Each rule is independent of any other and contains all information about itself.

For an overview of the Rules projects, please see the "Accessibility Rules" section in [Overview of Accessibility Insights for Windows](./Overview.md)

### Anatomy of a rule

Rules have three basic components

- `RuleInfo` 
   - `ID`: a unique value from the RuleId enumeration (used for telemetry and for rules to be run individually)
   - `Description`: a short description (fewer than 80 characters) of the standards violation (e.g., "The Name property must not be null")
   - `HowToFix`: an in-depth description of the standards violation and suggested remediation(s)
   - `Standard`: a value from the `A11yCriteriaId` enumeration which maps the given rule to a documented standard
   - `PropertyID`: an optional property id (e.g., `PropertyType.UIA_ControlTypePropertyId`) used to link a rule to a property-platform-based snippet with extended information about the rule
- `Condition`: a `Condition` object which determines under what circumstances the rule is run (see below)
- `Evaluate`: a method which determines if the test passes or is in violation of a standard

### Inheritence

All rules must inherit from the `Axe.Windows.Rules.Rule` base class. Rules are discovered through reflection; when your class inherits from `Rule`,  it is added to the set of rules tested by Accessibility Insights. 

### Conventions

#### One test per rule

A rule represents a single, self-contained test. For example, `NameIsNotNull`, `NameIsNotEmpty`, and `NameIsNotWhiteSpace` all test the value of the Name property in similar ways; but they are split into separate rules. This increases the specificity of the feedback given to the user and makes it possible to change the evaluation code or applicability (`Condition`) of each rule individually without unintentionally affecting other rules.

#### The `Evaluate` method returns only one failing `EvaluationCode`

The `Evaluate` method of a rule should return either `EvaluationCode.Pass` or only one of the following evaluation codes:

`EvaluationCode` | Description
--- | ---
`Error` | if the violation represents an unambiguous accessibility issue and can be conclusively determined by the tool
`Open` | if the violation can be conclusively determined by the tool but may or may not represent an accessibility issue
`Note` | if the violation cannot be conclusively determined by the tool and may or may not represent an accessibility issue
`Warning` | if the violation represents an unambiguous accessibility issue but cannot be conclusively determined by the tool
`RuleExecutionError` | if a problem occurred while executing the test

_Note:_ `EvaluationCode.NotApplicable` is never returned by the `Evaluate` method of the `Rule` class. It indicates that the rule in question is not applicable to the given situation. For example, a rule which checks for specific patterns on a button is not applicable to an edit control.

_Note:_ Because results from automated tests in Accessibility Insights are represented in the SARIF format, evaluation codes are loosely based on the "Level" property described in the [SARIF specification](http://docs.oasis-open.org/sarif/sarif/v2.0/csprd01/sarif-v2.0-csprd01.html##_Toc517436065) under section 3.19.7.

#### Use conditions in the `Evaluate` method

Using conditions (described below) makes it possible to represent the evaluation logic both as code and as a string that can be understood by a user. Please see the "Future" section of this document for more details.

### Conditions

Conditions are classses used to represent the properties, patterns, and values of an element in a grammatical form that is reusable, self-describing, and easy to read. All conditions must inherit from the `Axe.Windows.Rules.Condition` base class. Conditions have the following features:

- A condition evaluates to true or false via its `Matches` method.
- A condition has an associated text description. Descriptions can be assigned via the index operator, e.g., `Button["Button"]` 
- Conditions can be combined using operators:

operator | description | example
--- | --- | ---
& | logical and | `Button & Name.IsNotEmpty`
&#124; | logical or | `Button & Checkbox`
~ | unary logical not | `~IsContentElement`
&#45; | binary logical not | `Button - IsKeyboardFocusable`
/ | hierarchical relationship | `parent / child`

When conditions are combined using operators, so too are there associated descriptions, e.g., `(Button & IsKeyboardFocusable)` = "Button and IsKeyboardFocusable". Using this mechanism, all conditions can be written as text strings and used as documentation for the rules.

There are many ready-made conditions you can use from the `Axe.Windows.Rules.PropertyConditions` namespace.

### Future development

Ideally, all rules would be modified so that instead of an `Evaluate` method, they would contain an `Evaluation` condition property and a single possible `EvaluationCode` property to be returned in the case of a violation. This would make it possible (through the self-describing mechanism of conditions) to document every part of a rules logic and information. Exposing this information would make it easier to have conversations about exactly when rules apply, what evaluation logic should be performed, and what the severity of a violation should be. Such information has historically been either opaque, out-of-date, or non-existent. But it would be helpful to have discussion starting from a place where all the rules information is easily available without looking in the code.
