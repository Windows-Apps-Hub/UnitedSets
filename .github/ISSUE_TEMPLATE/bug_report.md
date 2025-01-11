---
name: Bug report
about: Create a report to help us improve
title: "[Bug] "
labels: bug
assignees: ''
body:
  - type: markdown
    attributes:
      value: "## Bug Report"
  - type: textarea
    id: bug-description
    attributes:
      label: Description
      description: Describe your bug here
      placeholder: |
          Ex. Settings button not working...
          If applicable, please attached a screenshot.
    validations:
      required: true
  - type: textarea
    id: reproduce
    attributes:
      label: To Reproduce
      description: If you can provide this, it might be helpful.
      placeholder: |
        1. Go to '...'
        2. Click on '....'
        3. See error |
        If applicable, please attached a screenshot.
    validations:
      required: false
