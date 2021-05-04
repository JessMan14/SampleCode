import React, { Component } from 'react'
import "./HistoricalData.css";

import ReactSuperSelect from 'react-super-select';

import Graph from './Graph'

export default class GraphContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
              location: {
                width: props.width + "px",
                height: props.height + "px",
                left: props.left + "px",
                top: props.top + "px"
            },
            context: props.context,
            data: null,
            loaded: false
        }
    }
    
      render() {
        let divStyle = {
            '--configured-width': this.props.width,
            '--configured-height': this.props.height + "px",
            '--configured-top': this.props.top + "px",
            '--configured-left': this.props.left
          };

        return (
           

            <div className="widget-box" style={divStyle}>
                 <div className="widget-title">Historical Data</div>
                 <div className="HistGraph-dropdown">
                        <ReactSuperSelect 
                        placeholder={"Electrical"}
                        customOptionTemplateFunction={this.renderItem}
                        dataSource={this.state.hierarchy}
                        onChange={this.handleClick}
                        optionLabelKey="label"
                        searchable={false}
                        groupBy="group" />        
                    </div>
                <div className="HistGraph-widgetContainer">
                    <Graph context={this.props.context} />                 
            </div>
        </div>      
        );
      }
    }
    
   
