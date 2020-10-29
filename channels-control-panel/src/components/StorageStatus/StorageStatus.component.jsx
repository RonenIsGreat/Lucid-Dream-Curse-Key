import React, { Component } from 'react';

import './StorageStatus.styles.scss';
import CanvasJSReact from '../../canvasjs.react';
import socketIOClient from 'socket.io-client';

// var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;
class StorageStatus extends Component {
    state = {
        drives: []
	}
	
	componentDidMount() {
		const ENDPOINT = "http://127.0.0.1:4000";
		const socket = socketIOClient(ENDPOINT);
		socket.on("StorageStatus", data => {
			let dataObject = JSON.parse(data);
			console.log(dataObject);
			this.setState({
				drives: dataObject
			})
		});
	};

	returnOptions = (drive) => ({
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
			name: "Used",
			showInLegend: true,
			indexLabel: "{y}",
			indexLabelFontColor: "white",
			yValueFormatString: "#,###'%'",
			dataPoints: [
				{ label: drive.name, y: drive.used / (drive.used + drive.available) * 100 }
			]
		}, {
			type: "stackedBar100",
			color: "#32CD32",
			name: "Available",
			showInLegend: true,
			indexLabel: "{y}",
			indexLabelFontColor: "white",
			yValueFormatString: "#,###'%'",
			dataPoints: [
				{ label: drive.name, y: drive.available / (drive.used + drive.available) * 100 },
			]
		}]
	})

	render() {
		return (
			<div className="storage-status">
				<h1>Drive Capacity Storage</h1>
				<br />
				{this.state.drives.map(drive => {
					return (
						<div key={drive.name+"heading"}>
							<h4>Drive: {drive.name}</h4>
							<h5>Total Space:      {(drive.used + drive.available).toFixed(2)} GB</h5>
							<h5>Used Space:       {drive.used.toFixed(2)} GB</h5>
							<h5>Available Space:  {drive.available.toFixed(2)} GB</h5>
						</div>
					);
				})}

				{this.state.drives.map(drive => {
					const options = this.returnOptions(drive);
					return (<CanvasJSChart key={drive.name} options={options} onRef={ref => this.chart = ref} />);
				})}
				{/*You can get reference to the chart instance as shown above using onRef. This allows you to access all chart properties and methods*/}
			</div>
		);
	}
}

export default StorageStatus;

