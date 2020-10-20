import Axios from 'axios';
import React, {Component} from 'react';

import Checkbox from '../Checkbox/Checkbox.component';
import './ChannelController.styles.scss';


const CHANNELS = [
    'CasBeam',

    'CasStave',
 
    'FasTasBeam',

    'FasTasStave',
  
    'PRSStave',
  
    'IDRSBus',
  
];

class ChannelController extends Component {
  state = {
    checkboxes: CHANNELS.reduce(
      (channels, channel) => ({
        ...channels,
        [channel]: this.props[channel]
      }),
      {}
    ),

    activeChannelsList: []
  };

  selectAllCheckboxes = isSelected => {
    Object.keys(this.state.checkboxes).forEach(checkbox => {
      this.setState(prevState => ({
        checkboxes: {
          ...prevState.checkboxes,
          [checkbox]: isSelected
        }
      }));
    });
  };

  selectAll = () => this.selectAllCheckboxes(true);

  deselectAll = () => this.selectAllCheckboxes(false);

  handleCheckboxChange = changeEvent => {
    const { name } = changeEvent.target;

    this.setState(prevState => ({
      checkboxes: {
        ...prevState.checkboxes,
        [name]: !prevState.checkboxes[name]
      }
    }));
  };

  handleFormSubmit = formSubmitEvent => {
    formSubmitEvent.preventDefault();

    this.setState({activeChannelsList: []});

    Object.keys(this.state.checkboxes)
      .filter(checkbox => this.state.checkboxes[checkbox])
      .forEach(checkbox => {
        this.state.activeChannelsList.push(checkbox);
      });

      console.log(this.state.activeChannelsList);

      Axios.post('http://localhost:3391/api/channel/PostActiveList', this.state.activeChannelsList);
  };

  createCheckbox = channel => (
    <Checkbox
      label={channel}
      isSelected={this.state.checkboxes[channel]}
      onCheckboxChange={this.handleCheckboxChange}
      key={channel}
    />
  );

  createCheckboxes = () => CHANNELS.map(this.createCheckbox);

  render() {
    return (
      <div className="channel-controller container">
        <div className="row mt-5">
          <div className="col-sm-12">
          <h1>Tsalul Channel Controller</h1>
          <h2>Select channels to listen to:</h2>
            <form onSubmit={this.handleFormSubmit}>
              {this.createCheckboxes()}

              <div className="form-group mt-2">
                <button
                  type="button"
                  className="btn btn-outline-primary mr-2"
                  onClick={this.selectAll}
                >
                  Select All
                </button>
                <button
                  type="button"
                  className="btn btn-outline-primary mr-2"
                  onClick={this.deselectAll}
                >
                  Deselect All
                </button>
                <button type="submit" className="btn btn-primary">
                  Accept
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    );
  }
}

export default ChannelController;


  

  

