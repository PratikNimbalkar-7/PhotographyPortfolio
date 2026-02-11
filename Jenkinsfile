pipeline {
			    agent any
			    stages {
			        stage('Build') {
			            steps {
			                echo 'Building...'
			            }
			        }
			        stage('SonarQube Analysis') {
			            steps {
			                script {
			                    // Assign tool inside script block
			                    def scannerHome = tool 'SonarScanner for MSBuild'
			                    // Use withSonarQubeEnv inside script block
			                    withSonarQubeEnv('MySonarQube') {
			                        bat "\"${scannerHome}\\SonarScanner.MSBuild.exe\" begin /k:\"WebAppDemo\""
			                        bat "dotnet build"
			                        bat "\"${scannerHome}\\SonarScanner.MSBuild.exe\" end"
			                    }
			                }
			            }
			        }
			        stage('Test') {
			            steps {
			                echo 'Testing...'
			            }
			        }
			        stage('Deploy') {
			            steps {
			                echo 'Deploying...'
			            }
			        }
			    }
			}
