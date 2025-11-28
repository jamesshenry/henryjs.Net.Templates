# Implementation Plan Template: Extract [YourFeature] Subcomponents

This is a template for planning the extraction of UI subcomponents into reusable parts.

## Overview

Extract [describe what you're extracting] from embedded [current implementation] into separate reusable [component type] components. Pass data ([data types]) directly in constructors for simplicity—no child ViewModels needed. Parent [ParentComponent] loads data and passes it; subcomponents handle display and raise selection events to parent.

## Getting Started

1. Replace `[YourFeature]` with your specific feature name
2. Replace component and data type placeholders
3. Add specific file paths and implementation details
4. Customize the steps for your architecture

## Architecture

- **[SubComponent1]**: Takes `[DataType1]` in constructor, displays in [UIControl], raises `[Event1]` event
- **[SubComponent2]**: Takes `[DataType2]` in constructor, displays in [UIControl], raises `[Event2]` event
- **[ParentViewModel]**: Loads data in `[InitializationMethod]`, passes to sub-views
- **[ParentView]**: Instantiates sub-views with data, handles events for [action]

## Detailed Steps

- [ ] **Create [SubComponent1]**
  - [ ] **Location**: `[YourFilePath1]`
  - [ ] **Details**:
    - [ ] Inherits [BaseClass]
    - [ ] Constructor takes `[DataType1] data`
    - [ ] Creates [UIControl] with [columns/fields]
    - [ ] Binds to passed data
    - [ ] Handles selection, raises `[Event1]` event

- [ ] **Create [SubComponent2]**
  - [ ] **Location**: `[YourFilePath2]`
  - [ ] **Details**:
    - [ ] Inherits [BaseClass]
    - [ ] Constructor takes `[DataType2] data`
    - [ ] Creates [UIControl] with [columns/fields]
    - [ ] Binds to passed data
    - [ ] Handles selection, raises `[Event2]` event

- [ ] **Update [ParentViewModel]**
  - [ ] **Location**: `[YourViewModelPath]`
  - [ ] **Details**:
    - [ ] Keep `[InitializationMethod]` loading data
    - [ ] No changes needed beyond data loading

- [ ] **Update [ParentView]**
  - [ ] **Location**: `[YourViewPath]`
  - [ ] **Details**:
    - [ ] Replace embedded components with new subcomponents
    - [ ] Instantiate with data from ViewModel
    - [ ] Subscribe to sub-view events, handle for [actions]

- [ ] **Register New Components in DI**
  - [ ] **Location**: `[YourDIPath]`
  - [ ] **Details**: Add registrations for new components

- [ ] **Verify Integration**
  - [ ] **Details**:
    - [ ] Build and test
    - [ ] Ensure subcomponents work correctly
    - [ ] Check reusability

## Dependencies

- [Describe step dependencies for your implementation]

## Success Criteria

- [List success criteria for your implementation]
