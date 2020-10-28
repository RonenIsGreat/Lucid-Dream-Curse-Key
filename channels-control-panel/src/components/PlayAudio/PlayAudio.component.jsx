import React from 'react';
import AudioPlayer from 'react-h5-audio-player';
import 'react-h5-audio-player/lib/styles.css';
import './PlayAudio.styles.scss'

const PlayAudioPage = () => {
    return (
        <div className="not-found">
           <AudioPlayer autoPlay src="https://file-examples-com.github.io/uploads/2017/11/file_example_MP3_700KB.mp3"></AudioPlayer>
        </div>
    );
}

export default PlayAudioPage;