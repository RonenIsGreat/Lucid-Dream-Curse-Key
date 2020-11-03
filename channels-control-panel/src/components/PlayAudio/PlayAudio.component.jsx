import React, {Component} from 'react';
import AudioPlayer from 'react-h5-audio-player';
import 'react-h5-audio-player/lib/styles.css';
import './PlayAudio.styles.scss'

class PlayAudioPage extends Component {
    state = {
        FileID: "",
        src: ""
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.setState({
            src: this.state.FileID
        });
        console.log(this.state);
    }

    handleChange = (e) => {
        this.setState({
            [e.target.id]: e.target.value
        });
    }

    render() {
        return (
            <div className="not-found">
                <form onSubmit={this.handleSubmit}>
                    <label htmlFor="FileID" className="label" >Enter File's ID : </label>
                    <br></br>
                    <input type="text" id="FileID" name="FileID" onChange={this.handleChange}></input>
                    <br></br>
                    <br></br>
                    <button type="submit">Play</button>
                </form>
                <AudioPlayer src={`${this.state.src}.wav`}></AudioPlayer>
            </div>
        );
    }
}

export default PlayAudioPage;