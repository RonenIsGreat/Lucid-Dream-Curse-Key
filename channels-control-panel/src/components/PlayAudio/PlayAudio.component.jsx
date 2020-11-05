import React, { Component } from 'react';
import AudioPlayer from 'react-h5-audio-player';
import 'react-h5-audio-player/lib/styles.css';
import './PlayAudio.styles.scss'
import Autosuggest from 'react-autosuggest';
import IsolatedScroll from 'react-isolated-scroll';
import axios from 'axios';

const autoCompleteTheme = {
    suggestionsContainer: {
        'background-color': 'white',
        'height': '10vh',
        'overflow': 'auto'
    },
    suggestion: {
        'cursor': 'pointer'
    },
    input: {
        'margin-bottom': '5px'
    }
};

// When suggestion is clicked, Autosuggest needs to populate the input
// based on the clicked suggestion. Teach Autosuggest how to calculate the
// input value for every given suggestion.
const getSuggestionValue = suggestion => suggestion;

// Use your imagination to render suggestions.
const renderSuggestion = suggestion => (
    <div>
        {suggestion}
    </div>
);

class PlayAudioPage extends Component {

    constructor() {
        super();

        axios.defaults.headers.post['Content-Type'] = 'application/json';
        axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';

        this.state = {
            FileID: "",
            suggestions: [],
            fileNames: []
        }
        this.fetchNames();
    }

    // Teach Autosuggest how to calculate suggestions for any given input value.
    getSuggestions(value) {
        const inputValue = value.trim().toLowerCase();
        const inputLength = inputValue.length;

        return inputLength === 0 ? this.state.fileNames : this.state.fileNames.filter(name =>
            name.toLowerCase().slice(0, inputLength) === inputValue
        );
    };

    // Autosuggest will call this function every time you need to update suggestions.
    // You already implemented this logic above, so just use it.
    onSuggestionsFetchRequested = ({ value }) => {
        this.setState({
            suggestions: this.getSuggestions(value)
        });
    };


    onChange = (event, { newValue }) => {
        this.setState({
            FileID: newValue
        });
    };

    fetchNames() {
        axios.get("http://localhost:5555/api/Channel/GetFileNames")
            .then((response) => {
                this.setState({
                    fileNames: response.data
                });
            })
        // Catch any errors we hit and update the app
    }

    renderSuggestionsContainer({ containerProps, children }) {
        const { ref, ...restContainerProps } = containerProps;
        const callRef = isolatedScroll => {
            if (isolatedScroll !== null) {
                ref(isolatedScroll.component);
            }
        };

        return (
            <IsolatedScroll ref={callRef} {...restContainerProps}>
                {children}
            </IsolatedScroll>
        );
    }

    render() {

        // Autosuggest will pass through all these props to the input.
        const inputProps = {
            placeholder: 'Type a file name',
            value: this.state.FileID,
            onChange: this.onChange
        };

        return (
            <div className="not-found">
                <form onSubmit={this.handleSubmit}>
                    <label htmlFor="FileID" className="label" >Enter File's ID : </label>
                    <br></br>
                    <Autosuggest
                        renderSuggestionsContainer={this.renderSuggestionsContainer}
                        theme={autoCompleteTheme}
                        suggestions={this.state.suggestions}
                        onSuggestionsFetchRequested={this.onSuggestionsFetchRequested}
                        getSuggestionValue={getSuggestionValue}
                        renderSuggestion={renderSuggestion}
                        inputProps={inputProps}
                        alwaysRenderSuggestions
                    />
                </form>
                <br></br>
                <AudioPlayer src={`${this.state.FileID}.wav`}></AudioPlayer>
            </div>
        );
    }
}

export default PlayAudioPage;