import React from 'react';
import AudioPlayer from 'react-h5-audio-player';
import 'react-h5-audio-player/lib/styles.css';
import './PlayAudio.styles.scss'

const PlayAudioPage = () => {

    return (
        <div className="not-found">
            <AudioPlayer autoPlay src="Sample.wav" ></AudioPlayer>
        </div>
    );
}

export default PlayAudioPage;