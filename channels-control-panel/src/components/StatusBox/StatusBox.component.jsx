import React from 'react';
import './StatusBox.styles.scss';

export default function StatusBox(props) {
    let status = props.status;

    return (
    <div className="status-box container">
        <div className={"row status-row " + status}>
            <div className="col">
                {props.name}
            </div>
        </div>
    </div>
    )
}