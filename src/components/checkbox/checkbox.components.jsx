import React from "react";

import "./checkbox.styles.css"

const Checkbox = ({ label, isSelected, onCheckboxChange }) => (
  <div className="form-check">
    <label>
      <input
        type="checkbox"
        name={label}
        checked={isSelected}
        onChange={onCheckboxChange}
        className="form-check-input"
      />
      <span>{label}</span>
    </label>
  </div>
);

export default Checkbox;