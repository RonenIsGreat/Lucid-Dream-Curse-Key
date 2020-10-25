import React, { Component } from 'react';

import './StorageStatus.styles.scss';
import CanvasJSReact from '../../canvasjs.react';
import socketIOClient from 'socket.io-client';

var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;
class StorageStatus extends Component {
    state = {
        used: 85,
        available: 15
	}
	
	componentDidMount() {
		const ENDPOINT = "http://127.0.0.1:4000";
		const socket = socketIOClient(ENDPOINT);
		socket.on("StorageStatus", data => {
			let dataObject = JSON.parse(data);
			this.setState({
				used : dataObject.used,
				available : dataObject.available
			})
			}
		)
	};

	render() {
		const options = {
			title: {
				text: ""
			},
			toolTip: {
				shared: true
			},
			legend: {
				verticalAlign: "top"
			},
			axisY: {
				suffix: "%"
			},
			data: [{
				type: "stackedBar100",
				color: "#FF0000",
				name: "used",
				showInLegend: true,
				indexLabel: "{y}",
				indexLabelFontColor: "white",
				yValueFormatString: "#,###'%'",
				dataPoints: [
					{ label: "Drive 1",   y: this.state.used }
				]
			},{
				type: "stackedBar100",
				color: "#32CD32",
				name: "Available",
				showInLegend: true,
				indexLabel: "{y}%",
				indexLabelFontColor: "white",
				yValueFormatString: "#,###'%'",
				dataPoints: [
					{ label: "Drive 1",   y: this.state.available },
				]
			}]
		}
		return (
		<div className="storage-status">
            <h1>Drive Capacity Storage</h1>
            <br/>
            <h5>Total Space:      100 GB</h5>
            <h5>Used Space:       {this.state.used} GB</h5>
            <h5>Available Space:  {this.state.available} GB</h5>

			<CanvasJSChart options = {options}
				/* onRef={ref => this.chart = ref} */
			/>
			{/*You can get reference to the chart instance as shown above using onRef. This allows you to access all chart properties and methods*/}
		</div>
		);
	}
}

export default StorageStatus;

